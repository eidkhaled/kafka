using Microsoft.AspNetCore.Mvc;

namespace kafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaController : ControllerBase
    {
        private readonly KafkaProducer _kafkaProducer;
        private readonly KafkaConsumer _kafkaConsumer;

        public KafkaController(KafkaProducer kafkaProducer, KafkaConsumer kafkaConsumer)
        {
            _kafkaProducer = kafkaProducer;
            _kafkaConsumer = kafkaConsumer;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string message)
        {
            await _kafkaProducer.ProduceAsync(message);

            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            _kafkaConsumer.Consume(message =>
            {
                Console.WriteLine($"Received message: {message}");
            });

            return Ok();
        }
    }
}
