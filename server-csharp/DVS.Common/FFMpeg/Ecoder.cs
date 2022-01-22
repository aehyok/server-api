using Lychee.Core.Infrastructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DVS.Common.FFMpeg
{
    public class Ecoder
    {
        public static string executeFFmpegCmd(string cmdArg,string ffmpegPath)
        {
            try
            {
                //#设置参数以直接输出图像序列(帧)
                ProcessStartInfo cmd = new ProcessStartInfo(ffmpegPath, cmdArg);
                cmd.RedirectStandardError = true; //set false
                cmd.RedirectStandardOutput = true; //set false
                cmd.UseShellExecute =false; //set true
                cmd.CreateNoWindow = true;  //don't need the black window

                Process process = new Process();
                process.StartInfo = cmd;
                process.Start();

                string errorMessage = process.StandardError.ReadToEnd();

                process.WaitForExit();
                process.Close();

                System.Threading.Thread.Sleep(100);
                return errorMessage;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        } 
        public static VideoFile GetVideoInfo(string filePath,string ffmpegPath)
        {
            VideoFile input = new VideoFile();
            string cmdArg = string.Format($"-i  { filePath}");
            string output = executeFFmpegCmd(cmdArg,ffmpegPath);

            input.RawInfo = output;
            input.Duration = ExtractDuration(input.RawInfo);
            input.BitRate = ExtractBitrate(input.RawInfo);
            input.RawAudioFormat = ExtractRawAudioFormat(input.RawInfo);
            input.AudioFormat = ExtractAudioFormat(input.RawAudioFormat);
            input.RawVideoFormat = ExtractRawVideoFormat(input.RawInfo);
            input.VideoFormat = ExtractVideoFormat(input.RawVideoFormat);
            input.Width = ExtractVideoWidth(input.RawInfo);
            input.Height = ExtractVideoHeight(input.RawInfo);
            input.FrameRate = ExtractFrameRate(input.RawVideoFormat);
            input.TotalFrames = ExtractTotalFrames(input.Duration, input.FrameRate);
            input.AudioBitRate = ExtractAudioBitRate(input.RawAudioFormat);
            input.VideoBitRate = ExtractVideoBitRate(input.RawVideoFormat);
            input.infoGathered = true;
            return input;
        }


        #region Extraction methods
        private static TimeSpan  ExtractDuration(string rawInfo)
        {
            TimeSpan t = new TimeSpan(0);
            Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)", RegexOptions.Compiled);
            Match m = re.Match(rawInfo);

            if (m.Success)
            {
                string duration = m.Groups[1].Value;
                string[] timepieces = duration.Split(new char[] { ':', '.' });
                if (timepieces.Length == 4)
                {
                    t = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
                }
            }

            return t;
        }
        private static double ExtractBitrate(string rawInfo)
        {
            Regex re = new Regex("[B|b]itrate:.((\\d|:)*)", RegexOptions.Compiled);
            Match m = re.Match(rawInfo);
            double kb = 0.0;
            if (m.Success)
            {
                Double.TryParse(m.Groups[1].Value, out kb);
            }
            return kb;
        }
        private static string ExtractRawAudioFormat(string rawInfo)
        {
            string a = string.Empty;
            Regex re = new Regex("[A|a]udio:.*", RegexOptions.Compiled);
            Match m = re.Match(rawInfo);
            if (m.Success)
            {
                a = m.Value;
            }
            return a.Replace("Audio: ", "");
        }
        private static string ExtractAudioFormat(string rawAudioFormat)
        {
            string[] parts = rawAudioFormat.Split(new string[] { ", " }, StringSplitOptions.None);
            return parts[0].Replace("Audio: ", "");
        }
        private static string ExtractRawVideoFormat(string rawInfo)
        {
            string v = string.Empty;
            Regex re = new Regex("[V|v]ideo:.*", RegexOptions.Compiled);
            Match m = re.Match(rawInfo);
            if (m.Success)
            {
                v = m.Value;
            }
            return v.Replace("Video: ", ""); ;
        }
        private static string ExtractVideoFormat(string rawVideoFormat)
        {
            string[] parts = rawVideoFormat.Split(new string[] { ", " }, StringSplitOptions.None);
            return parts[0].Replace("Video: ", "");
        }
        private static int ExtractVideoWidth(string rawInfo)
        {
            int width = 0;
            Regex re = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);
            Match m = re.Match(rawInfo);
            if (m.Success)
            {
                int.TryParse(m.Groups[1].Value, out width);
            }
            return width;
        }
        private static int ExtractVideoHeight(string rawInfo)
        {
            int height = 0;
            Regex re = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);
            Match m = re.Match(rawInfo);
            if (m.Success)
            {
                int.TryParse(m.Groups[2].Value, out height);
            }
            return height;
        }
        private static double ExtractFrameRate(string rawVideoFormat)
        {
            string[] parts = rawVideoFormat.Split(new string[] { ", " }, StringSplitOptions.None);

            double dFPS = 0;

            foreach (string p in parts)
            {
                if (p.ToLower().Contains("fps"))
                {
                    Double.TryParse(p.ToLower().Replace("fps", "").Replace(".", ",").Trim(), out dFPS);

                    break;
                }
                else if (p.ToLower().Contains("tbr"))
                {
                    Double.TryParse(p.ToLower().Replace("tbr", "").Replace(".", ",").Trim(), out dFPS);

                    break;
                }
            }

            //Audio: mp3, 44100 Hz, 2 channels, s16, 140 kb/s

            return dFPS;
        }
        private static double ExtractAudioBitRate(string rawAudioFormat)
        {
            string[] parts = rawAudioFormat.Split(new string[] { ", " }, StringSplitOptions.None);

            double dABR = 0;

            foreach (string p in parts)
            {
                if (p.ToLower().Contains("kb/s"))
                {
                    Double.TryParse(p.ToLower().Replace("kb/s", "").Replace(".", ",").Trim(), out dABR);

                    break;
                }
            }

            return dABR;
        }
        private static double ExtractVideoBitRate(string rawVideoFormat)
        {
            string[] parts = rawVideoFormat.Split(new string[] { ", " }, StringSplitOptions.None);

            double dVBR = 0;

            foreach (string p in parts)
            {
                if (p.ToLower().Contains("kb/s"))
                {
                    Double.TryParse(p.ToLower().Replace("kb/s", "").Replace(".", ",").Trim(), out dVBR);

                    break;
                }
            }

            return dVBR;
        }
        private static long ExtractTotalFrames(TimeSpan duration, double frameRate)
        {
            return (long)Math.Round(duration.TotalSeconds * frameRate, 0);
        }
        #endregion
    }
}
