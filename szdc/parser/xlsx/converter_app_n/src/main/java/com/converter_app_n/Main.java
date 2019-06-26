package com.converter_app_n;

import java.io.File;

public class Main {

    public static void main(String[] args) throws ImportException {

        File folder = new File(args[0]);
        File[] listOfFiles = folder.listFiles();

        int tablesCount = 0;

        for (int i = 0; i < listOfFiles.length; i++) {
            if (listOfFiles[i].isFile()) {

                File file = new File(args[0] + '/' + listOfFiles[i].getName());
                System.out.println("Processing file: " + file.getName());
                tablesCount+= SzdcTrainParser.ParseTrain(file);
            }
        }

        System.out.println("Number of parsed tables: " + tablesCount);
    }
}
