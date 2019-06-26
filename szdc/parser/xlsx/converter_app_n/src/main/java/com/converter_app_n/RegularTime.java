package com.converter_app_n;

import lombok.Getter;

@Getter
public class RegularTime {

    private final int minute;
    private final boolean moreThan30Seconds;

    public RegularTime(int minute, boolean moreThan30Seconds) {
        this.minute = minute;
        this.moreThan30Seconds = moreThan30Seconds;
    }

    @Override
    public String toString() {
        return minute + " + 30 sec? -> " + moreThan30Seconds;
    }
}
