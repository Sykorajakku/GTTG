package com.converter_app_n;

import lombok.Getter;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.util.CellRangeAddress;

import java.util.ArrayList;
import java.util.List;

public class TrainProcessor {

    private final List<CellRangeAddress> mergedRegions;

    private Row currentRow;
    private Train currentTrain;

    private int stationNameColumnIndex = -1;
    private int arrivalTimeColumnIndex = -1;
    private int departureTimeColumnIndex = -1;
    private int endDeparture;
    private int endArrival;

    private List<MovementTime> arrivalTimes = new ArrayList<>();
    private List<MovementTime> departureTimes = new ArrayList<>();

    private int arrivalHour = 0;
    private int departureHour = 0;

    private List<String> trainDefinitionDescriptions = new ArrayList<>();

    @Getter
    private List<Train> parsedTrains = new ArrayList<>();

    private enum ProcessorState {
        ParsingSchedule,
        AwaitingTrainDefinition,
        AwaitingScheduleHeader
    }

    private ProcessorState processorState;

    public TrainProcessor(List<CellRangeAddress> mergedRegions) {

        this.mergedRegions = mergedRegions;
        this.processorState = ProcessorState.AwaitingTrainDefinition;
    }

    public void processRow(Row row) {

        currentRow = row;

        if (processorState == ProcessorState.AwaitingTrainDefinition) {
            TryParseTrainDefinition();
        }
        else if (processorState == ProcessorState.AwaitingScheduleHeader) {
            TryParseTrainHeader();
        }
        else if (processorState == ProcessorState.ParsingSchedule) {
        try {
                TryParseTrainScheduleEvent();
            }catch(Exception ex) {

                System.out.println("Unable to parse train: " + currentTrain.getTrainNumber());
                arrivalTimes.clear();
                departureTimes.clear();
                currentTrain = null;
                arrivalHour = 0;
                departureHour = 0;
                stationNameColumnIndex = departureTimeColumnIndex = arrivalTimeColumnIndex = -1;

                processorState = ProcessorState.AwaitingTrainDefinition;
            }
        }
    }

    private boolean TryParseTrainHeader() {

        endArrival = endDeparture = stationNameColumnIndex = arrivalTimeColumnIndex = departureTimeColumnIndex = -1;

        int index = 0;
        for (var iter = currentRow.cellIterator(); iter.hasNext(); index++) {

            var cell = iter.next();
            var trimmedValue = cell.getStringCellValue().trim();

            if (trimmedValue.equals("1")) {
                stationNameColumnIndex = cell.getColumnIndex();
            }
            if (trimmedValue.equals("5")) {
                arrivalTimeColumnIndex = cell.getColumnIndex();
            }
            if (trimmedValue.equals("7")) {
                departureTimeColumnIndex = cell.getColumnIndex();
            }
        }

        if (stationNameColumnIndex != -1
            && arrivalTimeColumnIndex != -1 && departureTimeColumnIndex != -1) {

            var rowIndex = currentRow.getRowNum();
            endArrival = alignColumnIndexToEndOfMergedCell(rowIndex, arrivalTimeColumnIndex);
            endDeparture = alignColumnIndexToEndOfMergedCell(rowIndex, departureTimeColumnIndex);
            processorState = ProcessorState.ParsingSchedule;
            return true;
        }
        return false;
    }

    private int alignColumnIndexToEndOfMergedCell(int rowNum, int columnIndex) {

        var mergedRegion = mergedRegions.stream().filter(p -> p.isInRange(rowNum, columnIndex)).findAny();
        if (mergedRegion.isPresent()) {
            return mergedRegion.get().getLastColumn();
        }
        return columnIndex;
    }

    private void TryParseTrainScheduleEvent(){

        var arr = arrivalTimeColumnIndex;
        var dep = departureTimeColumnIndex;
        var sta = stationNameColumnIndex;

        if (!TryParseTrainHeader()) {
            arrivalTimeColumnIndex = arr;
            departureTimeColumnIndex = dep;
            stationNameColumnIndex = 0;
        } else {
            stationNameColumnIndex = 0;
        }

        var cell = currentRow.getCell(stationNameColumnIndex);
        if (cell == null) return;

        var stationColumnString = cell.getStringCellValue();

        if (stationColumnString.isEmpty()
            || stationColumnString.startsWith("Strana")
            || stationColumnString.startsWith("Lok")
            || stationColumnString.startsWith("Vygenerováno")
            || stationColumnString.startsWith("Platí")
            || stationColumnString.startsWith("Pokračování")
            || stationColumnString.startsWith("1")) {
            return;
        }

        if (TrainType.TryGetTrainType(stationColumnString).isPresent()) {

            ScheduleCategorizer.categorizeSchedule(currentTrain.getStationDataRows(), arrivalTimes, departureTimes);
            parsedTrains.add(currentTrain);

            arrivalHour = 0;
            departureHour = 0;

            arrivalTimes.clear();
            departureTimes.clear();

            processorState = ProcessorState.AwaitingTrainDefinition;
            TryParseTrainDefinition();
            return;
        }

        int index = 0;
        while (!Character.isAlphabetic(stationColumnString.charAt(index))) {
            index++;
        }
        var station = stationColumnString.substring(index);

        MovementTime arrivalTime = null;
        MovementTime departureTime = null;

        var arrivalCell = currentRow.getCell(arrivalTimeColumnIndex);
        if (arrivalCell != null) {
            arrivalTime = getMovementArrivalTime(arrivalCell.getStringCellValue());
        }

        var departureCell = currentRow.getCell(departureTimeColumnIndex);
        if (departureCell != null) {
            departureTime = getMovementDepartureTime(departureCell.getStringCellValue());
        }

        if (arrivalTime != null) arrivalTimes.add(arrivalTime);
        if (departureTime != null) departureTimes.add(departureTime);
        currentTrain.getStationDataRows().add(new StationDataRow(station));
    }

    private MovementTime getMovementArrivalTime(String stringValue) {

            for (int i = 0; i < stringValue.length(); ++i) {

                var character = stringValue.charAt(i);
                if (!Character.isDigit(character) && character != ' ') {
                    stringValue = stringValue.replace(character, ' ');
                }
            }

            var timeString = stringValue.trim();
            var firstDigitIndex = getFirstDigitIndex(timeString);

            if (firstDigitIndex == -1) return null;
            timeString = timeString.substring(firstDigitIndex);

            if (timeString.indexOf(' ') == 2) {
                arrivalHour = Integer.parseInt(timeString.substring(0, 2));
                timeString = timeString.substring(3);
            }

            if (timeString.indexOf(' ') == 1) {
                arrivalHour = Character.getNumericValue(timeString.charAt(0));
                timeString = timeString.substring(2);
            }

            var minutes = Integer.parseInt(timeString.substring(0, 2));

            if (timeString.length() == 3) {
                return new MovementTime(arrivalHour, minutes, true);
            } else if (timeString.length() == 2) {
                return new MovementTime(arrivalHour, minutes, false);
            }

        return  null;
    }

    private int getFirstDigitIndex(String timeString) {

        for (int i = 0; i < timeString.length(); ++i) {

            if (Character.isDigit(timeString.charAt(i))) {
                return i;
            }
        }
        return -1;
    }


    private MovementTime getMovementDepartureTime(String stringValue) {

        for (int i = 0; i < stringValue.length(); ++i) {

            var character = stringValue.charAt(i);
            if (!Character.isDigit(character) && character != ' ') {
                stringValue = stringValue.replace(character, ' ');
            }
        }

        var timeString = stringValue.trim();
        var firstDigitIndex = getFirstDigitIndex(timeString);

        if (firstDigitIndex == -1) return  null;
        timeString = timeString.substring(firstDigitIndex);

        if (timeString.indexOf(' ') == 2) {
            departureHour = Integer.parseInt(timeString.substring(0, 2));
            timeString = timeString.substring(3);
        }

        if (timeString.indexOf(' ') == 1) {
            departureHour = Character.getNumericValue(timeString.charAt(0));
            timeString = timeString.substring(2);
        }

        var minutes = Integer.parseInt(timeString.substring(0, 2));

        if (timeString.length() == 3) {
            return new MovementTime(departureHour, minutes, true);
        } else if (timeString.length() == 2) {
            return new MovementTime(departureHour, minutes, false);
        }

        return  null;
    }

    private RegularTime getRegularTime(String stringValue) {

        var trimmed = stringValue.trim();

        if (trimmed.isEmpty()) return null;

        if (trimmed.length() == 1) {

            if (Character.isDigit(trimmed.charAt(0))) {
                return new RegularTime(Character.getNumericValue(trimmed.charAt(0)), false);
            }
        }
        else {

            var moreThan30Seconds = false;
            if (stringValue.charAt(stringValue.length() - 1) == '5') {
                moreThan30Seconds = true;
            }

            var end = moreThan30Seconds ? stringValue.length() -1 : stringValue.length();
            return new RegularTime(Integer.parseInt(stringValue.substring(0, end)), false);
        }

        return null;
    }

    private void TryParseTrainDefinition() {

        trainDefinitionDescriptions.clear();
        var trainFound = false;

        for (var iter = currentRow.cellIterator(); iter.hasNext(); ) {

            var cell = iter.next();
            var cellStringContent = cell.getStringCellValue();


            var trainType = TrainType.TryGetTrainType(cellStringContent);

            if (trainType.isPresent()) {

                var indexWhitespace = getFirstDigitIndex(cellStringContent);
                cellStringContent = cellStringContent.substring(indexWhitespace);
                var trainNumber = parseTrainNumber(cellStringContent);
                currentTrain = new Train(trainType.get(), trainNumber);

                int index = indexWhitespace + 1 + trainNumber.length();
                cellStringContent = index < cellStringContent.length() ? cellStringContent.substring(index) : "";
                trainFound = true;
            }

            if (!cellStringContent.isEmpty())
                trainDefinitionDescriptions.add(cellStringContent);
        }

        if (trainFound) {
            currentTrain.getDescriptions().addAll(trainDefinitionDescriptions);
            processorState = ProcessorState.AwaitingScheduleHeader;
        }
    }

    private String parseTrainNumber(String cellStringContent) {

        int lastNumberIndex = cellStringContent.length();
        for (int i = 0; i < cellStringContent.length(); ++i) {
            if (!Character.isDigit(cellStringContent.charAt(i))) {
               lastNumberIndex = i;
               break;
            }
        }
        return cellStringContent.substring(0, lastNumberIndex);
    }
}
