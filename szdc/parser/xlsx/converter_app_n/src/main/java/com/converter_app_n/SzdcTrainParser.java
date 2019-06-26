package com.converter_app_n;

import com.monitorjbl.xlsx.StreamingReader;
import org.apache.poi.ss.usermodel.Workbook;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.List;

public class SzdcTrainParser {

    public static int ParseTrain(File trainRailwaySchedulesFile) throws ImportException {

        try(Workbook workbook = StreamingReader.builder().open(trainRailwaySchedulesFile)) {

            var sheet = workbook.getSheet(ImportConstants.ScheduleSheetName);
            var rowIterator = RowIterator.create(workbook, ImportConstants.ScheduleSheetName);

            if (rowIterator == null) {
                throw new ImportException("Sheet " + ImportConstants.ScheduleSheetName + " was not found.");
            }

            var mergedRegions = new MergedRegions(trainRailwaySchedulesFile);
            var trainProcessor = new TrainProcessor(mergedRegions.getMergedRegions(ImportConstants.ScheduleSheetName));

            for (var row : sheet) {
                trainProcessor.processRow(row);
            }

            var root = new JSONObject();

            var trainsJSON = new JSONObject();
            for (var train : trainProcessor.getParsedTrains()) {

                int x = Integer.parseInt(train.getTrainNumber());
                System.out.println(x);

                var trainJSON = new JSONObject();
                trainJSON.put("trainType", train.getTrainType().toString());

                var scheduleJSON = new JSONArray();

                List<StationDataRow> dataRows = train.getStationDataRows();
                for (StationDataRow dataRow : dataRows) {

                    var stationStopJSON = new JSONObject();
                    stationStopJSON.put("stationName", dataRow.getStation().trim());

                    JSONObject arrivalTimeJSON = null;
                    var arrivalTime = dataRow.getArrivalTime();
                    if (arrivalTime != null) {
                        arrivalTimeJSON = new JSONObject();
                        arrivalTimeJSON.put("hours", arrivalTime.getHour());
                        arrivalTimeJSON.put("minutes", arrivalTime.getMinute());
                        arrivalTimeJSON.put("isAfter30seconds", arrivalTime.isMoreThan30Seconds());
                    }
                    stationStopJSON.put("arrivalTime", arrivalTimeJSON);

                    JSONObject departureTimeJSON = null;
                    var departureTime = dataRow.getDepartureTime();
                    if (departureTime != null) {
                        departureTimeJSON = new JSONObject();
                        departureTimeJSON.put("hours", departureTime.getHour());
                        departureTimeJSON.put("minutes", departureTime.getMinute());
                        departureTimeJSON.put("isAfter30Seconds", departureTime.isMoreThan30Seconds());
                    }
                    stationStopJSON.put("departureTime", departureTimeJSON);

                    scheduleJSON.add(stationStopJSON);
                }

                trainJSON.put("schedule", scheduleJSON);
                trainsJSON.put(train.getTrainNumber(), trainJSON);
            }

            root.put("trains", trainsJSON);

            try (var file = new FileWriter(trainRailwaySchedulesFile.getName().replaceAll("xlsx","json"))) {
                file.write(root.toJSONString());
            }

            return trainProcessor.getParsedTrains().size();
        }
        catch (IOException ex) {
            throw new ImportException("Unable to process file with train schedule: ", ex);
        }
    }
}
