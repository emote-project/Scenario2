package uk.ac.hw.emote.enercities;

import java.io.File;
import java.text.DecimalFormat;
import java.text.DecimalFormatSymbols;
import java.util.ArrayList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

public class TestClass {

	public static void hi(String argv[]) throws Exception
	{
		Database d = new Database();
		System.out.println(d.getConstructions().get("Suburban").getPrice());
		System.out.println(d.getConstructions().get("Park").getPrice());
		System.out.println(d.getConstructions().get("Park").getField().get(2));
		System.out.println(d.getUpgrades().get("Solar_Roofs").getCost());
		System.out.println(d.getUpgrades().get("Solar_Roofs").getId());
		System.out.println(d.getConstructions().get("Park").getId());
		System.out.println(d.getConstructions().get("Suburban").getId());
		
		
	}
}
