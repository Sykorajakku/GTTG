package com.converter_app_n;

import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;

import java.util.Iterator;

public class RowIterator implements Iterator<Row> {

    private final Iterator<Row> rowIterator;
    private Row currentRow;
    private int currentRowIndex;

    private RowIterator(Sheet sheet) {

        rowIterator = sheet.rowIterator();
        currentRowIndex = -1;
    }

    public static RowIterator create(Workbook workbook, String sheetName) {

        int sheetIndex = workbook.getSheetIndex(sheetName);
        return (sheetIndex == -1) ? null : new RowIterator(workbook.getSheet(sheetName));
    }

    @Override
    public boolean hasNext() {
        return rowIterator.hasNext();
    }

    @Override
    public Row next() {
        ++currentRowIndex;
        currentRow = rowIterator.next();
        return currentRow;
    }

    public int getCurrentRowIndex() {
        return currentRowIndex;
    }
}
