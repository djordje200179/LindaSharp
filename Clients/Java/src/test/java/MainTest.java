import com.djordjemilanovic.lindasharp.client.RemoteLinda;
import java.util.Arrays;

public class MainTest {
	public static void main(String[] args) {
		var linda = new RemoteLinda("localhost", 5001);
		while (!linda.isHealthy()) {
			System.out.println("Waiting for server...");
			try {
				Thread.sleep(1000);
			} catch (InterruptedException _) {}
		}

		var result = linda.get("fib", 101, null);
		System.out.println(Arrays.toString(result));
	}
}
