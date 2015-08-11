package neural_network_binary_options;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;

import org.encog.Encog;
import org.encog.engine.network.activation.ActivationSigmoid;
import org.encog.ml.data.MLData;
import org.encog.ml.data.MLDataPair;
import org.encog.ml.data.MLDataSet;
import org.encog.ml.data.basic.BasicMLDataSet;
import org.encog.neural.networks.BasicNetwork;
import org.encog.neural.networks.layers.BasicLayer;
import org.encog.neural.networks.training.propagation.resilient.ResilientPropagation;
 

public class MainClass {
	// Data input here!
	public static String csvURL_EURAUD = "src/EURAUD_H1_STO_RSI_4.csv";
	public static String csvURL_EURUSD = "src/EURUSD_H1_STO_RSI_4.csv";
	
	public static double XOR_INPUT[][] = { { 0.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 },
		{1.0, 0.0, 1.0 }, {1.0, 1.0, 1.0 } };
	
	public static double FINANCE_DATA_INPUT[][] = {
		{0.83265121,0.78193578,0.80190290,0.73834197,0.47910270,0.43535283,0.42266352,0.40995336},
		{0.78193578,0.80190290,0.73834197,0.73050398,0.43535283,0.42266352,0.40995336,0.36925646},
		{0.80190290,0.73834197,0.73050398,0.35028902,0.42266352,0.40995336,0.36925646,0.24759241},
		{0.73834197,0.73050398,0.35028902,0.48974359,0.40995336,0.36925646,0.24759241,0.20516204},
		{0.73050398,0.35028902,0.48974359,0.41564562,0.36925646,0.24759241,0.20516204,0.14949119},
		{0.35028902,0.48974359,0.41564562,0.52339813,0.24759241,0.20516204,0.14949119,0.16061510},
		{0.48974359,0.41564562,0.52339813,0.69918699,0.20516204,0.14949119,0.16061510,0.11697322},
		{0.41564562,0.52339813,0.69918699,0.80458515,0.14949119,0.16061510,0.11697322,0.14718309},
		{0.52339813,0.69918699,0.80458515,0.81818182,0.16061510,0.11697322,0.14718309,0.21243180},
		{0.69918699,0.80458515,0.81818182,0.64439877,0.11697322,0.14718309,0.21243180,0.13918478},
		{0.80458515,0.81818182,0.64439877,0.56848971,0.14718309,0.21243180,0.13918478,0.04473981},
		{0.81818182,0.64439877,0.56848971,0.57142857,0.21243180,0.13918478,0.04473981,0.00568447},
		{0.64439877,0.56848971,0.57142857,0.58135017,0.13918478,0.04473981,0.00568447,0.00807640}
		};
	
	public static double FINANCE_DATA_EXPECTED[][] = {
			{0.0},{1.0},{1.0},{1.0},{1.0},{1.0},{1.0},{1.0},{0.0},{1.0},{0.0},{0.0},{1.0}};
	public static double XOR_IDEAL[][] = { { 0.0 }, { 1.0 }, { 1.0 }, { 0.0 } };
	
	public static ArrayList<ArrayList<Double>> importCSV1;
	public static ArrayList<ArrayList<Double>> importCSV2;
	
	public static void fillData(String URL){
		importCSV1 = new ArrayList<ArrayList<Double>>();
		importCSV2 = new ArrayList<ArrayList<Double>>();
		File csvFile = new File(URL);
		System.out.println(csvFile);
		String splitOne = ";";
		BufferedReader fileReader = null;
		
		try
        {
			String line = "";
            fileReader = new BufferedReader(new InputStreamReader(new FileInputStream(URL),"UTF-8"));


			while ((line = fileReader.readLine()) != null){
	            //Create the file reader
	            
	            ArrayList<Double> subArrayCSV1 = new ArrayList<Double>();
	            ArrayList<Double> subArrayCSV2 = new ArrayList<Double>();
	            
	            line = fileReader.readLine();
	            String[] tokens = line.split(splitOne);
	            	
	            for (String token1:tokens[0].split(",")){
	            	subArrayCSV1.add(Double.parseDouble(token1));
	            	//System.out.println("ADDING :" + Double.parseDouble(token1));
	            	};
	            
	            importCSV1.add(subArrayCSV1);
	            
	            String[] token2 = tokens[1].split(",");
	            double parsedToken1 = Double.parseDouble(token2[0]);
	            double parsedToken2 = Double.parseDouble(token2[1]);
	            
	            if (parsedToken1 <= parsedToken2){
	            	subArrayCSV2.add(1.0);
	            	//System.out.println("ADDING : " + true);
	            }
	            else {
	            	subArrayCSV2.add(0.0);
	            	//System.out.println("ADDING : "+false);
	            }
	            importCSV2.add(subArrayCSV2);
            }
            
          }
          catch (Exception e) {
                e.printStackTrace();
            }
            finally
            {
                try {
                    fileReader.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
				
	}
	
	
	public static void main(String args[]) {
		
		fillData(csvURL_EURAUD); 
		//double[][] learnedDataArray = learnedData.toArray()
		
		// create a neural network, without using a factory
		BasicNetwork network = new BasicNetwork();
		network.addLayer(new BasicLayer(null,true,2));
		network.addLayer(new BasicLayer(new ActivationSigmoid(),true,3));
		network.addLayer(new BasicLayer(new ActivationSigmoid(),false,1));
		network.getStructure().finalizeStructure();
		network.reset();

		double[][] FINANCE_DATA_PARAMETERS = new double[importCSV1.size()][importCSV1.get(1).size()];
		for (int i = 0; i <= FINANCE_DATA_PARAMETERS.length-1; i++){
			for (int t = 0; t <= FINANCE_DATA_PARAMETERS[i].length-1; t++){
				FINANCE_DATA_PARAMETERS[i][t] = importCSV1.get(i).get(t);
			}
		}

		double[][] FINANCE_DATA_IDEAL = new double[importCSV2.size()][importCSV2.get(1).size()];
		for (int i = 0; i <= FINANCE_DATA_IDEAL.length-1; i++){
			for (int t = 0; t <= FINANCE_DATA_IDEAL[i].length-1; t++){
				FINANCE_DATA_IDEAL[i][t] = importCSV2.get(i).get(t);
			}
		}
		
		
		
		// PRINTING OUT INFORMATION
		/*
		System.out.println("Parameter Values: ");
		for (double[] element_l1 : FINANCE_DATA_PARAMETERS){
			for (double element_l2 : element_l1){
				System.out.println(element_l2);
			}
		}
		
		System.out.println("Ideal Values: ");
		for (double[] element_l1 : FINANCE_DATA_IDEAL){
			for (double element_l2 : element_l1){
				System.out.println(element_l2);
			}
		}
		*/
		// END PRINTING OUT INFORMATION
		
		// create training data
		MLDataSet trainingSet = new BasicMLDataSet(FINANCE_DATA_PARAMETERS, FINANCE_DATA_IDEAL);

		//MLDataSet trainingSetTrial = new BasicMLDataSet(l,m)
		// train the neural network
		final ResilientPropagation train = new ResilientPropagation(network, trainingSet);
	
		int epoch = 1;
	
		do {
			train.iteration();
			System.out.println("Epoch #" + epoch + " Error:" + train.getError());
			epoch++;
		} while(train.getError() > 0.249499);
		train.finishTraining();
	
		// test the neural network
		
		double successCounter = 0;
		double counter = 0;
		System.out.println("Neural Network Results:");
		for(MLDataPair pair: trainingSet ) {
			final MLData output = network.compute(pair.getInput());
			System.out.println(pair.getInput().getData(0) + "," + pair.getInput().getData(1)
					+ ", actual=" + output.getData(0) + ",ideal=" + pair.getIdeal().getData(0));
			
			if ((output.getData(0) < 0.5 && pair.getIdeal().getData(0) == 0.0) || (output.getData(0) >= 0.5 && pair.getIdeal().getData(0) == 1.0)){
				successCounter++;
			}
			counter++;
		}
		
		double successRate = successCounter / counter;
		System.out.println(successRate);
	
		Encog.getInstance().shutdown();
	}

}
