using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Anthems
{
    public partial class _Default : System.Web.UI.Page
    {
        private BlobStorageService _blobStorageService = new BlobStorageService();
        private CloudQueueService _queueStorageService = new CloudQueueService();

        private CloudBlobContainer getContainer()
        {
            return _blobStorageService.getCloudBlobContainer();
        }

        private CloudQueue getAnthemMakerQueue()
        {
            return _queueStorageService.getCloudQueue();
        }


        private string GetMimeType(string Filename)
        {
            try
            {
                string ext = Path.GetExtension(Filename).ToLowerInvariant();
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (key != null)
                {
                    string contentType = key.GetValue("Content Type") as String;
                    if (!String.IsNullOrEmpty(contentType))
                    {
                        return contentType;
                    }
                }
            }
            catch
            {
            }
            return "application/octet-stream";
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            if (upload.HasFile)
            {
                var ext = Path.GetExtension(upload.FileName);
                var fileName = Path.GetFileNameWithoutExtension(upload.FileName);
                var random = new Random();
                var randomNumber = random.Next(0, 999);
                var numberString = randomNumber.ToString();
                var name = string.Format("{0}-{1}{2}", fileName, numberString, ext);
                String path = "audio/" + name;
                var blob = getContainer().GetBlockBlobReference(path);
                blob.Properties.ContentType = GetMimeType(upload.FileName);
                blob.UploadFromStream(upload.FileContent);
                getAnthemMakerQueue().AddMessage(new CloudQueueMessage(System.Text.Encoding.UTF8.GetBytes(name)));
                System.Diagnostics.Trace.WriteLine(String.Format("*** WebRole: Enqueued '{0}'", path));
            }
        }

        public string getBlobTitle(Uri uri)
        {
          CloudBlockBlob blob = new CloudBlockBlob(uri);
          blob.FetchAttributes();
          return blob.Metadata["Title"];
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                AnthemDisplayControl.DataSource = from blob in getContainer().GetDirectoryReference("clips").ListBlobs() select new { Url = blob.Uri, Title = getBlobTitle(blob.Uri) };
                AnthemDisplayControl.DataBind();
            }
            catch (KeyNotFoundException keyNotFound)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Error : {0}", keyNotFound));
            }
            catch (InvalidOperationException invalidOperation)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Error : {0}", invalidOperation));
            }
            catch (Exception)
            {
            }
        }
    }
}
