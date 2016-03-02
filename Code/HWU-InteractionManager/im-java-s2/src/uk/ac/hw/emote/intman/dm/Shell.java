package uk.ac.hw.emote.intman.dm;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

public class Shell {
	
	
	public static void main(String[] args)  {
		try {
			startEnercities();
			
			Thread.sleep(20000);
			
			stopEnercities();
		} catch (IOException | InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}

	public static void stopEnercities() throws IOException, InterruptedException{
		// The batch file to execute
	    final File batchFile = new File("stopEnercities.bat");

	    // The output file. All activity is written to this file
	    //final File outputFile = new File(String.format("output_%tY%<tm%<td_%<tH%<tM%<tS.txt",
	    //    System.currentTimeMillis()));

	    // The argument to the batch file. 
	    final String argument = "";

	    // Create the process
	    final ProcessBuilder processBuilder = new ProcessBuilder(batchFile.getAbsolutePath(), argument);
	    // Redirect any output (including error) to a file. This avoids deadlocks
	    // when the buffers get full. 
	    processBuilder.redirectErrorStream(true);
	    //processBuilder.redirectOutput(outputFile);

	    // Add a new environment variable
	    //processBuilder.environment().put("message", "Example of process builder");

	    // Set the working directory. The batch file will run as if you are in this
	    // directory.
	    //processBuilder.directory(new File("work"));

	    // Start the process and wait for it to finish. 
	    final Process process = processBuilder.start();
	    final int exitStatus = process.waitFor();
	    System.out.println("Processed finished with status: " + exitStatus);
		
	}

	public static void startEnercities() throws IOException, InterruptedException{
		// The batch file to execute
	    final File batchFile = new File("startEnercities.bat");

	    // The output file. All activity is written to this file
	    //final File outputFile = new File(String.format("output_%tY%<tm%<td_%<tH%<tM%<tS.txt",
	    //    System.currentTimeMillis()));

	    // The argument to the batch file. 
	    final String argument = "";

	    // Create the process
	    final ProcessBuilder processBuilder = new ProcessBuilder(batchFile.getAbsolutePath(), argument);
	    // Redirect any output (including error) to a file. This avoids deadlocks
	    // when the buffers get full. 
	    processBuilder.redirectErrorStream(true);
	    //processBuilder.redirectOutput(outputFile);

	    // Add a new environment variable
	    //processBuilder.environment().put("message", "Example of process builder");

	    // Set the working directory. The batch file will run as if you are in this
	    // directory.
	    //processBuilder.directory(new File("work"));

	    // Start the process and wait for it to finish. 
	    final Process process = processBuilder.start();
	    final int exitStatus = process.waitFor();
	    System.out.println("Processed finished with status: " + exitStatus);
	}

	public static void startDRLAgent() throws IOException, InterruptedException{
		final File batchFile = new File("startDRLAgent.bat");
		final String argument = "";
		final ProcessBuilder processBuilder = new ProcessBuilder(batchFile.getAbsolutePath(), argument);
		processBuilder.redirectErrorStream(true);
		final Process process = processBuilder.start();
	    final int exitStatus = process.waitFor();
	    System.out.println("Processed finished with status: " + exitStatus);
	}
	
	
	
	
	
			
}
