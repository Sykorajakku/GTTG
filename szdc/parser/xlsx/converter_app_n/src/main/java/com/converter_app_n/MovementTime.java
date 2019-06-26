package com.converter_app_n;

import lombok.Getter;
import lombok.Setter;

@Getter
public class MovementTime {

    private final boolean moreThan30Seconds;
    private final int hour;
    private final int minute;

    public MovementTime(int hour, int minute, boolean moreThan30Seconds) {
        this.hour = hour;
        this.minute = minute;
        this.moreThan30Seconds = moreThan30Seconds;
    }

    public boolean isBeforeOrSame(MovementTime other) {

        if (hour < other.hour) return true;
        else if (hour > other.hour) return false;
        else {

            if (minute < other.minute) return true;
            else if (minute > other.minute) return false;
            else {

                if (!other.isMoreThan30Seconds() && isMoreThan30Seconds()) return false;
                return true;
            }
        }
    }

    @Override
    public String toString() {
        return hour + ":" + minute + " + 30 sec? -> " + moreThan30Seconds;
    }
}
