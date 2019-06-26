package com.converter_app_n;

import lombok.Getter;

import java.util.ArrayList;
import java.util.List;

@Getter
public class Train {

    private final TrainType trainType;
    private final String trainNumber;
    private final List<String> descriptions = new ArrayList<>();
    private final List<StationDataRow> stationDataRows = new ArrayList<>();

    public Train(TrainType trainType, String trainNumber) {
        this.trainType = trainType;
        this.trainNumber = trainNumber;
    }
}
