import com.djordjemilanovic.lindasharp.RemoteLinda
import kotlinx.coroutines.delay
import kotlinx.coroutines.runBlocking
import kotlin.time.Duration.Companion.seconds

fun main() = runBlocking {
    val linda = RemoteLinda("localhost", 5001)
    while (!linda.isHealthy()) {
        println("Waiting for server...")
        delay(1.seconds)
    }

    val result = linda.get("fib", 101, null)
    println(result.contentToString())
}