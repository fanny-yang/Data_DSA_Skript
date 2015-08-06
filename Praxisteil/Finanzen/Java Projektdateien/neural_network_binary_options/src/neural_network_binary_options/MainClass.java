package neural_network_binary_options;

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
	/**
	 * The ideal data necessary for XOR.
	 */
	public static double XOR_IDEAL[][] = { { 0.0 }, { 1.0 }, { 1.0 }, { 0.0 } };
	
	/**
	 * The main method.
	 * @param args No arguments are used.
	 */
	public static void main(final String args[]) {
	
		// create a neural network, without using a factory
		BasicNetwork network = new BasicNetwork();
		network.addLayer(new BasicLayer(null,true,2));
		network.addLayer(new BasicLayer(new ActivationSigmoid(),true,3));
		network.addLayer(new BasicLayer(new ActivationSigmoid(),false,1));
		network.getStructure().finalizeStructure();
		network.reset();
	
		// create training data
		MLDataSet trainingSet = new BasicMLDataSet(FINANCE_DATA_INPUT, FINANCE_DATA_EXPECTED);
	
		// train the neural network
		final ResilientPropagation train = new ResilientPropagation(network, trainingSet);
	
		int epoch = 1;
	
		do {
			train.iteration();
			System.out.println("Epoch #" + epoch + " Error:" + train.getError());
			epoch++;
		} while(train.getError() > 0.30);
		train.finishTraining();
	
		// test the neural network
		System.out.println("Neural Network Results:");
		for(MLDataPair pair: trainingSet ) {
			final MLData output = network.compute(pair.getInput());
			System.out.println(pair.getInput().getData(0) + "," + pair.getInput().getData(1)
					+ ", actual=" + output.getData(0) + ",ideal=" + pair.getIdeal().getData(0));
		}
	
		Encog.getInstance().shutdown();
	}

}
