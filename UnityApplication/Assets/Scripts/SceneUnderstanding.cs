using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Networking;
using System.Net.Http;
using System.Net;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using Windows.Media.Capture;
using Windows.Media.Devices.Core;
using Windows.Graphics.Imaging;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.Globalization;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls; 
#endif

using Microsoft.MixedReality.Toolkit.Audio;


//[RequireComponent(typeof(TextToSpeech))]
public class SceneUnderstanding : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    private MediaCapture _mediaCapture;
#endif

    [SerializeField]
    public TextToSpeech textToSpeech;


    //public GameObject sceneUnderstandingObject;
    private static String endpoint = "http://192.168.84.214:5000/upload";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ExecuteButton()
    {
#if ENABLE_WINMD_SUPPORT
        Debug.Log("BUTTON INFORMATION ---------------- CaptureScene Button Clicked");
        OutputAudio("Capturing scene");
        CaptureScene();
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private async void CaptureScene()
    {
        Debug.Log("capturing software bitmap"); 
        SoftwareBitmap _softwareBitmap = await CaptureSoftwareBitmap();
        Texture2D photoAsTexture = await SoftwareBitmapToTexture(_softwareBitmap);

        byte[] bytes = photoAsTexture.EncodeToJPG(70);
        Debug.Log("Image as bytes to string: " + bytes.ToString());
        StartCoroutine(SendImage(bytes, (result) =>
            {
                Debug.Log("Received scene description: " + result);
                OutputAudio(result);
            }));
    }

    /// Thank you Jannis for this code!
    // private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
    // {
    //     Debug.Log("MediaCapture failed!", $"Error:\n{errorEventArgs.Message}");
    // }
    private async Task<SoftwareBitmap> CaptureSoftwareBitmap()
    {
        _mediaCapture = new MediaCapture();
        await _mediaCapture.InitializeAsync();
        //Debug.Log("MediaCapture failed!", $"Error:\n{errorEventArgs.Message}");
        //_mediaCapture.Failed += MediaCapture_Failed;
        // Prepare and capture photo
        var lowLagCapture = await _mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
        var capturedPhoto = await lowLagCapture.CaptureAsync();
        var _softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;
        await lowLagCapture.FinishAsync();
        return _softwareBitmap;
    }

    private async Task<Texture2D> SoftwareBitmapToTexture(SoftwareBitmap softwareBitmap)
    {
         // 1. Get video frame
        try
        {
            byte[] data;
            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
                encoder.SetSoftwareBitmap(softwareBitmap);
                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception ex) { return new Texture2D(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight); }
                data = new byte[ms.Size];
                await ms.ReadAsync(data.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }
            Texture2D tex = new Texture2D(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
            tex.LoadImage(data);
            tex.Apply();

            //tex.Resize(tex.width*2, tex.height*2);
            return tex;
        }
        catch (Exception e)
        {
            Debug.Log("Error! Could not convert from SoftwareBitmap to Texture: " + e.ToString());
            return new Texture2D(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
        }
    }
#endif

    private void OutputAudio(String text)
    {
        Debug.Log("Called OutputAudio with text: " + text);
        //textToSpeech = sceneUnderstandingObject.GetComponent<TextToSpeech>();
        textToSpeech = this.GetComponent<TextToSpeech>(); 
        // Speak message
        textToSpeech.StartSpeaking(text);

        Debug.Log("Finished speaking");
    }

    // Code from: https://learn.microsoft.com/en-us/uwp/api/windows.media.speechsynthesis.speechsynthesizer?view=winrt-22621
    // private void OutputAudio(String text)
    // {
    //     MediaElement mediaElement = new MediaElement(); 
    //     // 4. Output text as audio
    //     // TODO: somehow connect to a correct mediaElement
    //     Debug.Log("Called OutputAudio with text: " + text);

    //     // The media object for controlling and playing audio.
    //     // MediaElement mediaElement = this.media;

    //     // The object for controlling the speech synthesis engine (voice).
    //     var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

    //     // Generate the audio stream from plain text.
    //     // TODO: add text here instead of hello world 
    //     SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync("Hello World");

    //     // Send the stream to the media object.
    //     mediaElement.SetSource(stream, stream.ContentType);
    //     mediaElement.Play();
    // }


    IEnumerator SendImage(byte[] data, Action<string> result)
    {
        // 2. Send image to backend

        using (UnityWebRequest request = new UnityWebRequest(endpoint))
        {
            request.SetRequestHeader("Content-Type", "text/plain");
            request.method = "POST";
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Unable to send the POST: " + request.error);
            }
            else
            {
                Debug.Log("Sent the data to the API successfully");
                if (request.downloadHandler.text != null)
                {
                    // 3. Await text as response
                    result(request.downloadHandler.text);
                }
            }

        }
    }
}
