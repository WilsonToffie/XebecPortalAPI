using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Included;
using Python.Runtime;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json.Linq;
using Azure;
using Azure.Storage.Blobs;

namespace XebecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeParserController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        public async Task<JObject> Test()
        {
            StringBuilder output = new StringBuilder("Running python\n");
            Console.WriteLine("Setting python Evnironment\n");

            //Setting up python environment
            await Installer.SetupPython();
            PythonEngine.Initialize();

            //Installing modules
            Installer.TryInstallPip();
            Installer.PipInstallModule("spacy==2.3.7");
            Installer.PipInstallModule("PyMuPDF");
            //Installer.PipInstallModule("spacy-look-data");

            dynamic spacy = PythonEngine.ImportModule("spacy");



            output.AppendLine("Done !! Installing Spacy");
            output.AppendLine($"Spacy version:{spacy.__version__}");

            dynamic nlp_model = spacy.load("nlp_model");

            string filename = "Alice Clark CV.pdf";



            string ResumeFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, @"CVs");

            if (Directory.Exists(ResumeFilePath))
            {
                Directory.CreateDirectory(ResumeFilePath);
            }

            // file is downloaded
            // check file download was success or not

            dynamic fname =
                $"{ResumeFilePath}/{filename}";
            StringBuilder text = new StringBuilder(ReadPDF(fname));

            // dynamic doc2 = fitz.open(fname);
            // foreach (dynamic page in doc2)
            // {
            //     text.Append(page.get_text());
            // }



            dynamic doc = nlp_model(text.ToString());
            string res = "{";

            foreach (dynamic ent in doc.ents)
            {
                res += $" \"{ent.label_}\" : \"{ent.text.ToString()}\",";
            }

            res = res.Substring(0, res.Length - 1);
            res += "}";

            var test = JObject.Parse(res);

            return test;

        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IActionResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile([FromForm] string url)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                //Download file from Blob storage

                AzureSasCredential credential = new AzureSasCredential(
                    "sp=racwdli&st=2022-02-28T08:30:27Z&se=2022-03-11T16:30:27Z&sv=2020-08-04&sr=c&sig=TE%2B2VCz%2B6KKFbYHIkQwxGPOYWVUtht3xBPYZ8bE3kH4%3D");
                BlobClient blobClient = new BlobClient(new Uri(url), credential, new BlobClientOptions());

                string downloadFilePath = Environment.CurrentDirectory + @"\Resumes";
                if (Directory.Exists(downloadFilePath))
                {
                    Directory.CreateDirectory(downloadFilePath);
                }


                //var res = await blobClient.DownloadToAsync(downloadFilePath);
                Console.WriteLine($">>>>>>>>>>>>>>>>>>res path{Environment.CurrentDirectory} \n\ndownloadFilePath {downloadFilePath}<<<<<<<<<<<<<<<<<");
                //return Ok("Resume bois");
                Uri uri = new Uri(url);
                string filename = System.IO.Path.GetFileName(uri.LocalPath);

               string filePath = $@"{downloadFilePath}\{filename}";

                var result = blobClient.DownloadTo(filePath); // file is downloaded
                                                              // check file download was success or not
                if (result.Status == 206 || result.Status == 200)
                {
                    //string ress = TestSpacy(text);
                    //return Ok($"{ress}");
                    // You would be knowing this by now
                    //return Ok(ReadPDF(filePath));
                    StringBuilder output = new StringBuilder("Running python\n");
                    Console.WriteLine();

                    Console.WriteLine("Running python\n");
                    //output.AppendLine( await SetupPython());
                    //Console.WriteLine();
                    //output.AppendLine(await InstallSpacy());


                    Console.WriteLine("Setting python Evnironment\n");

                    await Installer.SetupPython();
                    PythonEngine.Initialize();
                    dynamic sys = PythonEngine.ImportModule("sys");

                    Console.WriteLine("Done !! Setting python Evnironment\n");
                    output.AppendLine("Done !! Setting python Evnironment\n");
                    output.AppendLine($"Python version:{sys.version}");

                    await Installer.SetupPython();
                    Installer.TryInstallPip();
                    Installer.PipInstallModule("spacy==2.3.7");
                    //Installer.PipInstallModule("spacy-look-data");
                    PythonEngine.Initialize();
                    dynamic spacy = PythonEngine.ImportModule("spacy");


                    output.AppendLine("Done !! Installing Spacy");
                    output.AppendLine($"Spacy version:{spacy.__version__}");

                    dynamic nlp_model = spacy.load("nlp_model");
                    Installer.PipInstallModule("PyMuPDF");
                    dynamic fitz = PythonEngine.ImportModule("fitz");


                    dynamic fname =
                        $@"{downloadFilePath}\{filename}";
                    dynamic doc2 = fitz.open(fname);

                    StringBuilder text = new StringBuilder();

                    foreach (dynamic page in doc2)
                    {
                        text.Append(page.get_text());
                    }


                    dynamic doc = nlp_model(text.ToString());
                    string res = "{";

                    foreach (dynamic ent in doc.ents)
                    {
                        res += $" \"{ent.label_}\" : \"{ent.text.ToString()}\",";

                    }

                    res = res.Substring(0, res.Length - 1);
                    res += "}";
                    Console.WriteLine(res);

                    var test = JObject.Parse(res);

                    return Ok(test);
                }
                return Ok($"{result.Status}");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("upload2")]
        public async Task<IActionResult> UploadFile2([FromForm] string url)
        {
            try
            {


                StringBuilder output = new StringBuilder("Running python\n");
                Console.WriteLine();
                Console.WriteLine("Running python\n");
                //output.AppendLine( await SetupPython());
                //Console.WriteLine();
                //output.AppendLine(await InstallSpacy());
                Console.WriteLine("Setting python Evnironment\n");
                await Installer.SetupPython();
                PythonEngine.Initialize();
                dynamic sys = PythonEngine.ImportModule("sys");
                Console.WriteLine("Done !! Setting python Evnironment\n");
                output.AppendLine("Done !! Setting python Evnironment\n");
                output.AppendLine($"Python version:{sys.version}");
                await Installer.SetupPython();
                Installer.TryInstallPip();
                Installer.PipInstallModule("spacy==2.3.7");
                //Installer.PipInstallModule("spacy-look-data");
                PythonEngine.Initialize();
                dynamic spacy = PythonEngine.ImportModule("spacy");
                output.AppendLine("Done !! Installing Spacy");
                output.AppendLine($"Spacy version:{spacy.__version__}");
                dynamic nlp_model = spacy.load("nlp_model");
                Installer.PipInstallModule("PyMuPDF");
                dynamic fname = url;
                //dynamic doc2 = fitz.open(fname);
                StringBuilder text = new StringBuilder();
                dynamic doc = nlp_model(ReadPDF(fname));
                string res = "{";
                foreach (dynamic ent in doc.ents)
                {
                    res += $" \"{ent.label_}\" : \"{ent.text.ToString()}\",";
                }
                res = res.Substring(0, res.Length - 1);
                res += "}";
                Console.WriteLine(res);
                var test = JObject.Parse(res);
                return Ok(test);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
         [HttpPost("uploadtest")]
        public async Task<IActionResult> UploadTest([FromForm] string url)
        {
            try
            {


                StringBuilder output = new StringBuilder("Running python\n");
                Console.WriteLine();
                Console.WriteLine("Running python\n");
                //output.AppendLine( await SetupPython());
                //Console.WriteLine();
                //output.AppendLine(await InstallSpacy());
                Console.WriteLine("Setting python Evnironment\n");
                await Installer.SetupPython();
                PythonEngine.Initialize();
                dynamic sys = PythonEngine.ImportModule("sys");
                Console.WriteLine("Done !! Setting python Evnironment\n");
                output.AppendLine("Done !! Setting python Evnironment\n");
                output.AppendLine($"Python version:{sys.version}");
                await Installer.SetupPython();
                Installer.TryInstallPip();
                Installer.PipInstallModule("spacy==2.3.7");
                //Installer.PipInstallModule("spacy-look-data");
                PythonEngine.Initialize();
                dynamic spacy = PythonEngine.ImportModule("spacy");
                output.AppendLine("Done !! Installing Spacy");
                output.AppendLine($"Spacy version:{spacy.__version__}");
                dynamic nlp_model = spacy.load("nlp_model");
                Installer.PipInstallModule("PyMuPDF");
                //dynamic fname = url;
                //dynamic doc2 = fitz.open(fname);
                //StringBuilder text = new StringBuilder();
                //dynamic doc = nlp_model(ReadPDF(fname));
                //string res = "{";
                //foreach (dynamic ent in doc.ents)
                //{
                //    res += $" \"{ent.label_}\" : \"{ent.text.ToString()}\",";
                //}
                //res = res.Substring(0, res.Length - 1);
                //res += "}";
                //Console.WriteLine(res);
                //var test = JObject.Parse(res);
                return Ok(output);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
         [HttpPost("pythoneng")]
        public string PythonEngen()
        {
            try
            {
                string output = "";
                PythonEngine.Initialize();
                output += "Initializing python";
                dynamic sys = PythonEngine.ImportModule("sys");
                output += "Installing sys";
                return output;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
        }
        [HttpPost("PythonEngineSys")]
        public string Eng()
        {
            try
            {
                string output = "";
                PythonEngine.Initialize();
                output += "Initializing python";
                return output;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


        [HttpPost("Installer")]
        public async Task<IActionResult> Initguy()
        {
            try
            {
                string output = "";
                await Installer.SetupPython();
                output += "Running Installer";
                return Ok(output);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }

        }

        [HttpPost("InstallerPip")]
        public async Task<IActionResult> InstallPip()
        {
            try
            {
                string output = "";
                await Installer.SetupPython();
                output += "Running Installer";
                Installer.PipInstallModule("spacy==2.3.7");
                output += "Installing pip";
                return Ok(output);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }

        }


        private string ReadPDF(string filepath)
        {
            //string filePath = @"C:\Users\me\RiderProjects\Work Projects\Real\CV Resume\FixedsumeReader\ScanResume\ScanResume\Server\StaticFiles\Resumes\functionalSample.pdf";

            PdfReader reader = new PdfReader(filepath);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }

            reader.Close();

            return text;
        }

        [HttpGet("getallfiles")] //Please modify
        public string GetAllFiles()
        {
            try
            {

                string str = "";
                string resumefolder = Environment.CurrentDirectory;
                DirectoryInfo d = new DirectoryInfo(resumefolder); //Assuming Test is your Folder
                
                if (Directory.Exists(Environment.CurrentDirectory))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Resumes");
                }
                
                DirectoryInfo[] child = d.GetDirectories();

                foreach (DirectoryInfo file in child)
                {
                    str = str + "\n " + file.FullName;
                }

                str += str + "\n Files +\n ";

                FileInfo[] Files = d.GetFiles(); //Getting pdf files
            
                foreach (FileInfo file in Files)
                {
                    str = str + "\n " + file.Name;
                }
                return str;
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }

        [HttpGet("getCurrent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string GetEnv()
        {
            try
            {
                return Environment.CurrentDirectory;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        [HttpGet("GetBaseDir")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string GetBase()
        {
            try
            {
                return $"Physical location {AppDomain.CurrentDomain.BaseDirectory}";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        [HttpGet("AppContext")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public string GettAppBase()
        {
            try
            {
                return $"AppContext.BaseDir {AppContext.BaseDirectory}";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }



    }
}
