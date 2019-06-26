package com.converter_app_n;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.openxml4j.exceptions.OpenXML4JException;
import org.apache.poi.openxml4j.opc.OPCPackage;
import org.apache.poi.ss.util.CellRangeAddress;
import org.apache.poi.xssf.eventusermodel.XSSFReader;
import org.w3c.dom.NodeList;
import org.xml.sax.Attributes;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import org.xml.sax.XMLReader;
import org.xml.sax.helpers.DefaultHandler;
import com.monitorjbl.xlsx.XmlUtils;
import org.xml.sax.helpers.XMLReaderFactory;

import java.io.*;
import java.net.URI;
import java.util.*;

public class MergedRegions {

    private final File file;

    public MergedRegions(File file) {
        this.file = file;
    }

    public List<CellRangeAddress> getMergedRegions(String sheetName) throws ImportException {

        MergedRegionsLocator mergedRegionsLocator = new MergedRegionsLocator();

        try (InputStream inputStream = new FileInputStream(file)) {

            OPCPackage pkg = OPCPackage.open(inputStream);
            XSSFReader reader = new XSSFReader(pkg);

            int sheetIndex = getSheetIndex(reader, sheetName);
            XSSFReader.SheetIterator iter = (XSSFReader.SheetIterator) reader.getSheetsData();

            HashSet<URI> usedURIs = new HashSet<URI>();
            Map<Integer, InputStream> sheetStreams = new LinkedHashMap<>();

            int iterIndex = 0;

            while (iter.hasNext()) {

                InputStream is = iter.next();
                URI uri = iter.getSheetPart().getPartName().getURI();

                if (usedURIs.contains(uri)) {
                    is.close();
                }
                else {
                    usedURIs.add(uri);
                    sheetStreams.put(iterIndex, is);
                    iterIndex++;
                }
            }

            for (Map.Entry<Integer, InputStream> inputStreamEntry : sheetStreams.entrySet()) {
                if (inputStreamEntry.getKey() != sheetIndex) {
                    inputStreamEntry.getValue().close();
                }
            }

            try (InputStream sheetData = sheetStreams.get(sheetIndex)) {

                XMLReader parser = XMLReaderFactory.createXMLReader();
                parser.setContentHandler(mergedRegionsLocator);
                parser.parse(new InputSource(sheetData));
            }

        } catch (IOException | SAXException | OpenXML4JException ex) {
            throw new ImportException("Unable to get merge regions for sheet " + sheetName, ex);
        }

        return mergedRegionsLocator.getMergedRegions();
    }

    static class MergedRegionsLocator extends DefaultHandler {

        private final List<CellRangeAddress> mergedRegions = new ArrayList<>();

        @Override
        public void startElement(String uri, String localName, String name, Attributes attributes) {

            if ("mergeCell".equals(name) && attributes.getValue("ref") != null) {
                mergedRegions.add(CellRangeAddress.valueOf(attributes.getValue("ref")));
            }
        }

        public List<CellRangeAddress> getMergedRegions() { return new ArrayList<>(mergedRegions); }
    }

    private static int getSheetIndex(XSSFReader reader, String name) throws IOException, InvalidFormatException, ImportException {

        NodeList nodeList = XmlUtils.searchForNodeList(XmlUtils.document(reader.getWorkbookData()), "/workbook/sheets/sheet");

        for (int i = 0; i < nodeList.getLength(); i++) {

            if (nodeList.item(i).getAttributes().getNamedItem("name").getTextContent().equals(name)) {
                return i;
            }
        }
        throw new ImportException("Sheet " + name + " not found.");
    }
}
