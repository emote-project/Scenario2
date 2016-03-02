package uk.ac.hw.emote.intman.dm;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Hashtable;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.eclipse.jetty.util.log.Log;
import org.json.JSONException;
import org.json.JSONObject;



public class Translator {
	private static Translator _inst = new Translator();
	private Hashtable<String, JSONObject> symbolSWE, directionSWE, toolSWE, skillSWE;
	
	private Translator() {
		// Singleton
	}

	public static Translator getInstance() {
		return _inst;
	}

	public static void main(String[] arg){
		System.out.println("Tool translation");
		Translator.getInstance().init();
		JSONObject j = Translator.getInstance().translateTool("SWE", "mapKey");
		System.out.println("Tool:"+ j.toString());
	}
	
	public void init() {
		symbolSWE = new Hashtable<String,JSONObject>();
		directionSWE = new Hashtable<String,JSONObject>();
		toolSWE = new Hashtable<String,JSONObject>();
		skillSWE = new Hashtable<String,JSONObject>();
		
		initSymbols();
		initDirections();
		initTools();
		initSkills();
		
		Log.getLog().info("Translator initiated");
	}

	
	public JSONObject translateTool(String language, String s){
		s = s.toLowerCase();
		if (language.equals("SWE")){
			if (toolSWE.containsKey(s.toLowerCase())){
				return toolSWE.get(s);
			}
		} 
		else {
			if (s.equals("distance")){
				s = "measuring tool";
			} else if (s.equals("mapKey")){
				s = "map key";
			}
			JSONObject k = new JSONObject();
			try {
				k.put("/tool/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		}
		return null;
	}
	
	public JSONObject translateSkill(String language, String s){
		s = s.toLowerCase();
		if (language.equals("SWE")){
			if (skillSWE.containsKey(s.toLowerCase())){
				return skillSWE.get(s);
			}
		} else {
			if (s.equals("distance")){
				s = "measuring distances";
			} else if (s.equals("direction")){
				s = "cardinal directions";
			} else if (s.equals("symbol")){
				s = "identifying symbols";
			}
			JSONObject k = new JSONObject();
			try {
				k.put("/skill/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		}
		return null;
	}

	public JSONObject translateDirection(String language, String s){
		s = s.toLowerCase();
		if (language.equals("SWE")){
			if (directionSWE.containsKey(s.toLowerCase())){
				return directionSWE.get(s);
			}
		} else {
			JSONObject k = new JSONObject();
			try {
				k.put("/direction/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		}
		return null;
	}

	public JSONObject translateDistance(String language, String s){
		s = s.toLowerCase();
		if (language.equals("SWE")){
			s = s.replaceAll("\\.", " komma ");
			s = s.replace("kilometers", "kilometer");
			s = s.replace("meters", "meter");
			JSONObject k = new JSONObject();
			try {
				k.put("/distance/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		} 
		else {
			s = s.replaceAll("\\.", " point ");
			JSONObject k = new JSONObject();
			try {
				k.put("/distance/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		}
		
	}

	public JSONObject translateSymbol(String language, String s){
		s = s.toLowerCase();
		if (language.equals("SWE")){
			if (symbolSWE.containsKey(s.toLowerCase())){
				return symbolSWE.get(s);
			} else {
				JSONObject k = new JSONObject();
				try {
					k.put("/symbol/", s);
					k.put("/symbol_prenouned/", s);
					k.put("/symbol_terminated/", s);
				} catch (JSONException e) {
					e.printStackTrace();
				}
				return k;
			}
		} 
		else {
			JSONObject k = new JSONObject();
			try {
				k.put("/symbol/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		}
		
	}
	
	public JSONObject translateSymbol2(String language, String s){
		if (language.equals("SWE")){
			if (symbolSWE.containsKey(s.toLowerCase())){
				return symbolSWE.get(s);
			} else {
				JSONObject k = new JSONObject();
				try {
					k.put("/symbol/", s);
					k.put("/symbol_prenouned/", s);
					k.put("/symbol_terminated/", s);
				} catch (JSONException e) {
					e.printStackTrace();
				}
				return k;
			}
		} 
		else {
			JSONObject k = new JSONObject();
			try {
				k.put("/symbol/", s);
			} catch (JSONException e) {
				e.printStackTrace();
			}
			return k;
		}
		
	}
	
	private void initTools() {
		try {
			FileInputStream fileInputStream = new FileInputStream(new File("tagTranslation/Tools_SWE.xls"));
			HSSFWorkbook workbook = new HSSFWorkbook(fileInputStream);
			HSSFSheet worksheet = workbook.getSheet("Sheet1");
			
			int l = worksheet.getLastRowNum();
			
			
			for (int i=1; i <= l; i++){
				HSSFRow row = worksheet.getRow(i);
				HSSFCell cellA = row.getCell(0);
				String aVal = "";
				if (cellA == null){
					aVal = "";
				} else {
					aVal = (cellA.getStringCellValue()).toLowerCase();
				}
				
				JSONObject j = new JSONObject();
				
				HSSFCell cellB = row.getCell(1);
				String bVal = cellB.getStringCellValue();
				j.put("/tool/", bVal);
				
				HSSFCell cellC = row.getCell(2);
				String cVal = cellC.getStringCellValue();
				j.put("/tool_prenouned/", cVal);
				
				HSSFCell cellD = row.getCell(3);
				String dVal = cellD.getStringCellValue();
				j.put("/tool_terminated/", dVal);
				
				System.out.println("" + i + ";" + aVal + ";" + bVal + ";" + cVal + ";" + dVal);
				
				toolSWE.put(aVal, j);
			}
			
			workbook.close();
			System.out.println("Keys tools: " + toolSWE.size());
			
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
	private void initSkills() {
		try {
			FileInputStream fileInputStream = new FileInputStream(new File("tagTranslation/Skills_SWE.xls"));
			HSSFWorkbook workbook = new HSSFWorkbook(fileInputStream);
			HSSFSheet worksheet = workbook.getSheet("Sheet1");
			
			int l = worksheet.getLastRowNum();
			
			
			for (int i=1; i <= l; i++){
				HSSFRow row = worksheet.getRow(i);
				HSSFCell cellA = row.getCell(0);
				String aVal = "";
				if (cellA == null){
					aVal = "";
				} else {
					aVal = (cellA.getStringCellValue()).toLowerCase();
				}
				
				JSONObject j = new JSONObject();
				
				HSSFCell cellB = row.getCell(1);
				String bVal = cellB.getStringCellValue();
				j.put("/skill/", bVal);
				
				/*
				HSSFCell cellC = row.getCell(2);
				String cVal = cellC.getStringCellValue();
				j.put("/symbol_prenouned/", cVal);
				
				HSSFCell cellD = row.getCell(3);
				String dVal = cellD.getStringCellValue();
				j.put("/symbol_terminated/", dVal);
				*/
				
				System.out.println("" + i + ";" + aVal + ";" + bVal);
				
				skillSWE.put(aVal, j);
			}
			
			workbook.close();
			System.out.println("Keys skills: " + skillSWE.size());
			
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	private void initDirections() {
		try {
			FileInputStream fileInputStream = new FileInputStream(new File("tagTranslation/Directions_SWE.xls"));
			HSSFWorkbook workbook = new HSSFWorkbook(fileInputStream);
			HSSFSheet worksheet = workbook.getSheet("Sheet1");
			
			int l = worksheet.getLastRowNum();
			
			
			for (int i=1; i <= l; i++){
				HSSFRow row = worksheet.getRow(i);
				HSSFCell cellA = row.getCell(0);
				String aVal = "";
				if (cellA == null){
					aVal = "";
				} else {
					aVal = (cellA.getStringCellValue()).toLowerCase();
				}
				
				JSONObject j = new JSONObject();
				
				HSSFCell cellB = row.getCell(1);
				String bVal = cellB.getStringCellValue();
				j.put("/direction/", bVal);
				
				/*
				HSSFCell cellC = row.getCell(2);
				String cVal = cellC.getStringCellValue();
				j.put("/symbol_prenouned/", cVal);
				
				HSSFCell cellD = row.getCell(3);
				String dVal = cellD.getStringCellValue();
				j.put("/symbol_terminated/", dVal);
				*/
				
				System.out.println("" + i + ";" + aVal + ";" + bVal);
				
				directionSWE.put(aVal, j);
			}
			
			workbook.close();
			System.out.println("Keys direction: " + directionSWE.size());
			
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	private void initSymbols() {
		try {
			FileInputStream fileInputStream = new FileInputStream(new File("tagTranslation/Symbols_SWE.xls"));
			HSSFWorkbook workbook = new HSSFWorkbook(fileInputStream);
			HSSFSheet worksheet = workbook.getSheet("Sheet1");
			
			int l = worksheet.getLastRowNum();
			
			
			for (int i=1; i <= l; i++){
				HSSFRow row = worksheet.getRow(i);
				HSSFCell cellA = row.getCell(0);
				String aVal = "";
				if (cellA == null){
					aVal = "";
				} else {
					aVal = (cellA.getStringCellValue()).toLowerCase();
				}
				
				JSONObject j = new JSONObject();
				
				HSSFCell cellB = row.getCell(1);
				String bVal = cellB.getStringCellValue();
				j.put("/symbol/", bVal);
				
				HSSFCell cellC = row.getCell(2);
				String cVal = cellC.getStringCellValue();
				j.put("/symbol_prenouned/", cVal);
				
				HSSFCell cellD = row.getCell(3);
				String dVal = cellD.getStringCellValue();
				j.put("/symbol_terminated/", dVal);
				
				System.out.println("" + i + ";" + aVal + ";" + bVal + ";" + cVal + ";" + dVal);
				
				symbolSWE.put(aVal, j);
			}
			
			workbook.close();
			System.out.println("Keys symbols: " + symbolSWE.size());
			
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
}
