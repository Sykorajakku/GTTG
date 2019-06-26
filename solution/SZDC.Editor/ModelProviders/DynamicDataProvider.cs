using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using GTTG.Core.Time;
using GTTG.Model.Model.Events;
using SZDC.Editor.Interfaces;
using SZDC.Model.Events;

namespace SZDC.Editor.ModelProviders {
    
    /// <summary>
    /// Tool which manages registered timetables as receivers and provides them with current time and traffic.
    /// </summary>
    public class DynamicDataProvider {

        public DateTimeInterval CurrentTimeInterval { get; private set; }
        private const int IntervalHoursLength = 8;

        private readonly Random _random = new Random();
        private readonly IStaticDataProvider _staticDataProvider;
        private readonly ModelProvider _modelProvider;
        private readonly Dictionary<int, StaticTrainDescription> _staticSet; // loaded data (whole time schedule)
        private readonly Dictionary<int, HashSet<IDynamicDataReceiver>>_trainReceivers;
        private Dictionary<int, ImmutableArray<TrainEvent>> _workingSet; // currently used
        private readonly HashSet<IDynamicDataReceiver> _receivers;

        public DynamicDataProvider(IStaticDataProvider staticDataProvider,
                                   ModelProvider modelProvider) {

            CurrentTimeInterval = CreateCurrentInterval();
            _staticDataProvider = staticDataProvider;
            _modelProvider = modelProvider;
            _receivers = new HashSet<IDynamicDataReceiver>();
            _staticSet = new Dictionary<int, StaticTrainDescription>();
            _trainReceivers = new Dictionary<int, HashSet<IDynamicDataReceiver>>();
            _workingSet = new Dictionary<int, ImmutableArray<TrainEvent>>();
        }

        private DateTimeInterval CreateCurrentInterval() {
            
            var now = DateTime.Now;
            var start = now.Date.AddHours(now.Hour - IntervalHoursLength / 2);
            CurrentTimeInterval = new DateTimeInterval(start, start.AddHours(IntervalHoursLength));
            return CurrentTimeInterval;
        }

        /// <summary>
        /// Adds new timetable as receiver and initializes their content with connection to other receivers and their content.
        /// </summary>
        public void Register(IDynamicDataReceiver receiver) {

            var importedTrainNumbers = new HashSet<int>();
            var newTrains = new List<StaticTrainDescription>();
            var existingTrains = new List<StaticTrainDescription>();

            var trainsInRailwaySegment = _staticDataProvider.LoadTrainsInRailwaySegment(receiver.ReceiverDescription).ToList();
            
            foreach (var selectedTrain in trainsInRailwaySegment) {

                importedTrainNumbers.Add(selectedTrain.TrainNumber);
                if (!_staticSet.ContainsKey(selectedTrain.TrainNumber)) {
                    newTrains.Add(selectedTrain); // adding new train, attach all intersecting receivers
                }
                else {
                    existingTrains.Add(selectedTrain);  // add to existing list and attach this receiver to it
                }
            }

            foreach (var newTrain in newTrains) { // for each new train, initialize structures

                var trainReceivers = new HashSet<IDynamicDataReceiver> { receiver };

                var trainNumber = newTrain.TrainNumber;
                foreach (var otherReceiver in _receivers) { 

                    if (otherReceiver.ReceiverDescription.IntersectsWith(newTrain.StaticSchedule)) {
                        trainReceivers.Add(otherReceiver);
                    }
                }

                _staticSet.Add(trainNumber, newTrain); // add new train to static set
                _trainReceivers.Add(trainNumber, trainReceivers); // attach all it's receivers to new train
            }

            foreach (var existingTrain in existingTrains) {
                // add to static set of receivers
                _trainReceivers[existingTrain.TrainNumber].Add(receiver);
            }

            foreach (var staticTrain in _staticSet) {
                if (!importedTrainNumbers.Contains(staticTrain.Key) // not yet discovered
                    && receiver.ReceiverDescription.IntersectsWith(staticTrain.Value.StaticSchedule)) {

                    _trainReceivers[staticTrain.Key].Add(receiver); // attach this receiver to existing train
                }
            }
            
            _receivers.Add(receiver);
            TriggerUpdate();
        }

        /// <summary>
        /// Possibly moves current displayed schedule scope in time interval by one hour to match current <see cref="DateTime"/>.
        /// Removes trains out of scope, add new ones.
        /// </summary>
        public void TriggerUpdate() {

            var now = DateTime.Now;
            var trainsByReceiver = _receivers.ToDictionary(r => r, _ => new Dictionary<int, (StaticTrainDescription, ImmutableArray<TrainEvent>)>());
            var dateTimeInterval = CreateCurrentInterval();

            var oldWorkingSet = _workingSet;
            _workingSet = new Dictionary<int, ImmutableArray<TrainEvent>>();

            foreach (var trainEntry in oldWorkingSet) {
                if (dateTimeInterval.IntersectsWith(ConvertToInterval(trainEntry.Value))) {
                    _workingSet.Add(trainEntry.Key, trainEntry.Value);
                }

                foreach (var receiver in _trainReceivers[trainEntry.Key]) {
                    trainsByReceiver[receiver].Add(trainEntry.Key, (_staticSet[trainEntry.Key], trainEntry.Value));
                }
            }

            foreach (var staticTrain in _staticSet) {

                if (_workingSet.ContainsKey(staticTrain.Key)) {
                    continue; // train already displayed
                }

                DateTime start;

                // select current day or tomorrow intersection
                var todayInterval = _modelProvider.ConvertToDateTimeInterval(staticTrain.Value.StaticSchedule, now);
                var tomorrowInterval = new DateTimeInterval(todayInterval.Start.AddDays(1), todayInterval.End.AddDays(1));

                if (dateTimeInterval.IntersectsWith(todayInterval)) {
                    start = now;
                } else if (dateTimeInterval.IntersectsWith(tomorrowInterval)) {
                    start = now.AddDays(1);
                } else {
                    continue; // no intersection with datetime intervals
                }

                try {
                    var events = _modelProvider.ConvertToSchedule(staticTrain.Value.StaticSchedule, start);
                    _workingSet.Add(staticTrain.Key, events);
                    foreach (var receiver in _trainReceivers[staticTrain.Key]) {
                        trainsByReceiver[receiver].Add(staticTrain.Key, (staticTrain.Value, events));
                    }
                }
                catch (ModelDefinitionException) {
                    /*
                     *  TODO:
                     *  Currently not reporting errors when triggering update.
                     *  Implementation would report errors to receiver interface.
                     */
                }
            }

            foreach (var receiverEntry in trainsByReceiver) {
                receiverEntry.Key.Update(dateTimeInterval, receiverEntry.Value);
            }
        }

        private static DateTimeInterval ConvertToInterval(ImmutableArray<TrainEvent> events) {
            return new DateTimeInterval(events[0].DateTime, events.Last().DateTime);
        }

        /// <summary>
        /// Unsubscribe receiver. If trains are not in use by other receivers, release them.
        /// </summary>
        public void RemoveReceiver(IDynamicDataReceiver receiver) {

            _receivers.Remove(receiver);
            var toRemove = new List<int>();

            foreach (var trainReceiversPair in _trainReceivers) {

                // only receiver for particular train number ==> remove train entries
                if (trainReceiversPair.Value.Contains(receiver)) {

                    if (trainReceiversPair.Value.Count != 1) { // remove all train if only one receiver
                        trainReceiversPair.Value.Remove(receiver);
                    }
                    else { // remove only entry in train receivers
                        toRemove.Add(trainReceiversPair.Key);
                    }
                }
            }

            foreach (var trainNumber in toRemove) {
                _staticSet.Remove(trainNumber);
                _workingSet.Remove(trainNumber);
                _trainReceivers.Remove(trainNumber);
            }
        }

        /// <summary>
        /// Modify track under <paramref name="trainNumber"/> with new schedule.
        /// If <paramref name="trainNumber"/> not presents, nothing happens.
        /// </summary>
        public void Modify(int trainNumber, ImmutableArray<TrainEvent> schedule) {

            if (!_workingSet.ContainsKey(trainNumber)) return;

            foreach (var receiver in _trainReceivers[trainNumber]) {
                _workingSet[trainNumber] = schedule;
                receiver.Modify(trainNumber, schedule);
            }
        }

        /// <summary>Picks random train from current trains and tries to modify it's event from future.</summary>
        public void ModifySchedule() {

            if (_workingSet.Count == 0) return;

            // select train with index
            var index = _random.Next(0, _workingSet.Count - 1);
            var key = _workingSet.Keys.ElementAt(index);
            var schedule = _workingSet[key];
            var firstModifiableIndex = GetFirstEventInFuture(schedule);

            if (firstModifiableIndex != schedule.Length) {
                var modifiedEventIndex = _random.Next(firstModifiableIndex, schedule.Length - 1);
                var modifiedEvent = schedule[modifiedEventIndex];

                // start window where event datetime can be moved
                var modifyWindowStart = modifiedEventIndex == firstModifiableIndex ? DateTime.Now : schedule[modifiedEventIndex - 1].DateTime;
                // end window where event datetime can be moved (if last event modified, add 10 minutes max
                var modifyWindowEnd = modifiedEventIndex == schedule.Length - 1 ? modifiedEvent.DateTime.AddMinutes(10) : schedule[modifiedEventIndex + 1].DateTime;

                // select random multiply from window timespan
                var seconds = Math.Max(0, (int)(modifyWindowEnd - modifyWindowStart).TotalSeconds);
                var millisecondsToAdd = _random.Next(0, seconds);
                var newDateTime = modifyWindowStart.AddSeconds(millisecondsToAdd);
                var newEvent = ((SzdcTrainEvent) modifiedEvent).Clone(newDateTime);

                // Update schedule, receivers updated
                schedule = schedule.Replace(modifiedEvent, newEvent);
                _workingSet[key] = schedule;

                foreach (var receiver in _trainReceivers[key]) {
                    receiver.Modify(key, schedule);
                }
            }
        }

        private static int GetFirstEventInFuture(ImmutableArray<TrainEvent> schedule) {

            for (var i = 0; i < schedule.Length; ++i) {
                if (schedule[i].DateTime >= DateTime.Now) {
                    return i;
                }
            }
            return schedule.Length;
        }
    }
}
