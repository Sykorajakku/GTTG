// Copyright (c) Jakub Sýkora. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

using GTTG.Core.Base;
using GTTG.Core.Drawing.Canvases;
using GTTG.Core.Strategies.Interfaces;

namespace GTTG.Core.Strategies.Implementations {

    /// <summary>Represents storage of registered elements under placement type which are mapped to particular segment type.</summary>
    /// <typeparam name="TPlacementType">Type of placement type.</typeparam>
    /// <typeparam name="TElement">Type of element of <see cref="IVisual"/>.</typeparam>
    /// <typeparam name="TSegmentType">Type of segment type.</typeparam>
    public class BasicStrategyManager<TPlacementType, TElement, TSegmentType>
        : Visual, IReadOnlyDictionary<TPlacementType, TElement>
        where TElement : IVisual {

        /// <inheritdoc/>
        public TElement this[TPlacementType key] => Elements[key];

        /// <inheritdoc/>
        public IEnumerable<TPlacementType> Keys => Elements.Keys;

        /// <inheritdoc/>
        public IEnumerable<TElement> Values => Elements.Values;

        /// <inheritdoc/>
        public int Count => Elements.Count;

        /// <summary>Maps registered <typeparamref name="TPlacementType"/> types to <typeparamref name="TSegmentType"/>.</summary>
        public IReadOnlyDictionary<TPlacementType, TSegmentType> ManagedSegmentTypes => SegmentTypes;

        /// <summary>Collection of registered <typeparamref name="TElement"/> under <typeparamref name="TPlacementType"/>.</summary>
        protected readonly Dictionary<TPlacementType, TElement> Elements;
        
        /// <summary>Collection of registered <typeparamref name="TElement"/> under <typeparamref name="TSegmentType"/>.</summary>
        protected readonly Dictionary<TPlacementType, TSegmentType> SegmentTypes;
        
        /// <summary>Converter between <typeparamref name="TPlacementType"/> and <typeparamref name="TSegmentType"/> values.</summary>
        protected readonly ITypeConverter<TPlacementType, TSegmentType> TypeConverter;

        /// <summary>
        /// Creates empty basic strategy manager.
        /// </summary>
        /// <param name="typeConverter">Converter for placement type to segment type.</param>
        public BasicStrategyManager(ITypeConverter<TPlacementType, TSegmentType> typeConverter) {

            TypeConverter = typeConverter;
            Elements = new Dictionary<TPlacementType, TElement>();
            SegmentTypes = new Dictionary<TPlacementType, TSegmentType>();
        }

        /// <inheritdoc />
        public override bool HasHit(SKPoint contentPoint) {
            return Elements.Count != 0;
        }

        /// <inheritdoc />
        public override IEnumerable<IVisual> ProvideVisuals() {
            foreach (var element in Elements.Values) {
                yield return element;
            }
        }

        /// <inheritdoc />
        public override void Draw(DrawingCanvas drawingCanvas) {

            foreach (var element in Elements.Values) {
                drawingCanvas.Draw(element);
            }
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TPlacementType, TElement>> GetEnumerator() {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public bool ContainsKey(TPlacementType key) {
            return Elements.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(TPlacementType key, out TElement value) {
            return Elements.TryGetValue(key, out value);
        }

        /// <summary>
        /// Clears all added elements and cleans held resources and registrations.
        /// </summary>
        public virtual void Clear() {
            Elements.Clear();
            SegmentTypes.Clear();
        }

        /// <summary>
        /// Determines if elem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TPlacementType, TElement> item) {
            return Elements.Contains(item);
        }

        /// <summary>
        /// Adds element to manager under key.
        /// </summary>
        /// <param name="key">Placement type of added element.</param>
        /// <param name="value">Element to be added.</param>
        /// <exception cref="StrategyException">Element under the key already registered.</exception>
        public virtual void Add(TPlacementType key, TElement value) {

            if (Elements.ContainsKey(key)) {
                throw new StrategyException($"Item {key} is already in manager");
            }

            var segmentType = TypeConverter.Convert(key);
            Elements.Add(key, value);
            SegmentTypes.Add(key, segmentType);
        }


        /// <summary>Adds element to manager under key wrapped as <see cref="KeyValuePair{TKey,TValue}"/>.</summary>
        /// <exception cref="StrategyException">Element under the key already registered.</exception>
        public virtual void Add(KeyValuePair<TPlacementType, TElement> item) {
            Add(item.Key, item.Value);
        }

        /// <summary>Removes item from manager and removes all it's registrations and resources.</summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>True if removed; otherwise false.</returns>
        public virtual bool Remove(KeyValuePair<TPlacementType, TElement> item) {

            if (TryGetValue(item.Key, out var value)) {
                if (value.Equals(item.Value)) {
                    RemoveSafe(item.Key);
                    return true;
                }
            }
            return false;
        }

        /// <summary>Removes item from manager and removes all it's registrations and resources.</summary>
        /// <param name="key">Item to remove under key.</param>
        /// <returns>True if removed; otherwise false.</returns>
        public virtual bool Remove(TPlacementType key) {
            if (!Elements.ContainsKey(key)) return false;
            RemoveSafe(key);
            return true;
        }

        private void RemoveSafe(TPlacementType key) {
            Elements.Remove(key);
            SegmentTypes.Remove(key);
        }
    }
}
