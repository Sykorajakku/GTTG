package com.converter_app_n;

import lombok.Getter;
import java.util.Optional;

@Getter
public enum TrainType {

    OS("Os"),
    Ex("Ex"),
    R("R"),
    Sp("Sp"),
    Nex("Nex"),
    Pn("Pn"),
    Mn("Mn"),
    Vl("Vl"),
    Sl("Sl");

    private final String description;

    TrainType(String description) {
        this.description = description;
    }

    public static Optional<TrainType> TryGetTrainType(String string) {

        for (var value : TrainType.values()) {
            if (string.startsWith(value.description + ' ')) {
                return Optional.of(value);
            }
        }

        if (string.contains("Vleč ")) {
            return Optional.of(Vl);
        }

        return Optional.empty();
    }

    @Override
    public String toString() {
        return description;
    }
}
