using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XiboClient2.Settings;

namespace XiboClient2.Processes
{
    public class RenderMedia
    {

        public RenderMedia()
        {
            // MediaNodeList = new List<MediaOption>();
        }

        public static void MediaList(XmlNodeList listMedia, MediaOption _options)
        {
            //Read Media list one by one
            foreach (XmlNode mediaNode in listMedia)
            {
                XmlAttributeCollection nodeAttributes = mediaNode.Attributes;
                ParseOptionsForMediaNode(mediaNode, nodeAttributes, _options);
            }
        }

        /// <summary>
        /// Get Media options
        /// </summary>
        /// <param name="mediaNode"></param>
        /// <param name="nodeAttributes"></param>
        /// <param name="_options"></param>
        private static void ParseOptionsForMediaNode(XmlNode mediaNode, XmlAttributeCollection nodeAttributes, MediaOption _options)
        {
            //Media Id
            _options.mediaId = int.Parse(nodeAttributes["id"].Value);

            // Type and Duration will always be on the media node
            _options.type = nodeAttributes["type"].Value;

            // Render as
            if (nodeAttributes["render"] != null)
                _options.render = nodeAttributes["render"].Value;

            //TODO: Check the type of node we have, and make sure it is supported.

            if (nodeAttributes["duration"].Value != "")
            {
                _options.duration = int.Parse(nodeAttributes["duration"].Value);
            }
            else
            {
                _options.duration = 60;
            }

            // We cannot have a 0 duration here... not sure why we would... but
            if (_options.duration == 0 && _options.type != "video" && _options.type != "localvideo")
            {
                //int emptyLayoutDuration = int.Parse(ApplicationSettings.Default.EmptyLayoutDuration.ToString());
                //_options.duration = (emptyLayoutDuration == 0) ? 10 : emptyLayoutDuration;
            }

            // There will be some stuff on option nodes
            XmlNode optionNode = mediaNode.SelectSingleNode("options");
            foreach (XmlNode option in optionNode.ChildNodes)
            {
                if (option.Name == "direction")
                {
                    _options.direction = option.InnerText;
                }
                else if (option.Name == "uri")
                {
                    _options.uri = option.InnerText;
                }
                else if (option.Name == "loop")
                {
                    _options.loop = int.Parse(option.InnerText);
                }
                else if (option.Name == "mute")
                {
                    _options.mute = int.Parse(option.InnerText);
                }
                else if (option.Name == "copyright")
                {
                    _options.copyrightNotice = option.InnerText;
                }
                else if (option.Name == "scrollSpeed")
                {
                    if (option.InnerText != null && option.InnerText != "")
                    {
                        _options.scrollSpeed = int.Parse(option.InnerText);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Non integer scrollSpeed in XLF", "Region - SetNextMediaNode");
                    }
                }
                else if (option.Name == "updateInterval")
                {
                    //updateIntervalProvided = true;

                    if (option.InnerText != null && option.InnerText != "")
                    {
                        _options.updateInterval = int.Parse(option.InnerText);
                    }
                    else
                    {
                        // Update interval not defined, so assume a high value
                        _options.updateInterval = 3600;
                    }
                }
                else if (option.Name == "modeid")
                {
                    //updateIntervalProvided = true;

                    if (option.InnerText != null && option.InnerText != "")
                    {
                        _options.modeid = option.InnerText;
                    }
                    else
                    {
                        // Update interval not defined, so assume a high value
                        _options.modeid = "";
                    }
                }
                //Transition details
                else if (option.Name == "transIn")
                {
                    _options.transIn = option.InnerText;
                }
                else if (option.Name == "transInDirection")
                {
                    _options.transInDirection = option.InnerText;
                }
                else if (option.Name == "transInDuration")
                {
                    _options.transInDuration = double.Parse(option.InnerText);
                }
                else if (option.Name == "transOut")
                {
                    _options.transOut = option.InnerText;
                }
                else if (option.Name == "transOutDirection")
                {
                    _options.transOutDirection = option.InnerText;
                }
                else if (option.Name == "transOutDuration")
                {
                    _options.transOutDuration = double.Parse(option.InnerText);
                }
                //Shell Command Details
                else if (option.Name == "launchThroughCmd")
                {
                    _options.launchThroughCmd = option.InnerText;
                }
                else if (option.Name == "name")
                {
                    _options.scName = option.InnerText;
                }
                else if (option.Name == "terminateCommand")
                {
                    _options.terminateCommand = option.InnerText;
                }
                else if (option.Name == "useTaskkill")
                {
                    _options.useTaskkill = option.InnerText;
                }
                else if (option.Name == "windowsCommand")
                {
                    _options.windowsCommand = option.InnerText;
                }
            }

            // And some stuff on Raw nodes
            XmlNode rawNode = mediaNode.SelectSingleNode("raw");
            if (rawNode != null)
            {
                foreach (XmlNode raw in rawNode.ChildNodes)
                {
                    if (raw.Name == "text")
                    {
                        _options.text = raw.InnerText;
                    }
                    else if (raw.Name == "template")
                    {
                        _options.documentTemplate = raw.InnerText;
                    }
                    else if (raw.Name == "embedHtml")
                    {
                        _options.text = raw.InnerText;
                    }
                    else if (raw.Name == "embedScript")
                    {
                        _options.javaScript = raw.InnerText;
                    }
                }
            }

            // Audio Nodes?
            XmlNodeList audio = mediaNode.SelectNodes("audio");

            if (audio != null && audio.Count > 0)
            {
                AudioOption _audio = new AudioOption();
                foreach (XmlNode audioNode in audio)
                {
                    if (audioNode.HasChildNodes)
                    {
                        _audio.audioUrl = PlayerSettings.libraryPath + audioNode.InnerText;
                        if (audioNode.Attributes["loop"] != null)
                        {
                            _audio.loop = int.Parse(audioNode.Attributes["loop"].Value);
                        }

                        if (audioNode.Attributes["volume"] != null)
                        {
                            _audio.volume = int.Parse(audioNode.Attributes["volume"].Value);
                        }

                        RenderLayout.AudioNodeList.Add(
                            new AudioOption(
                                _options.mediaId,
                                _audio.volume,
                                _audio.loop,
                                _audio.audioUrl,
                                _options.regionId
                                )
                            );
                    }

                    _options.audio = true;
                }
            }

            CreateNextMediaNode(_options);
        }

        /// <summary>
        /// Add Media Details in to MedaiList
        /// </summary>
        /// <param name="options"></param>
        private static void CreateNextMediaNode(MediaOption options)
        {
            //Check media type
            if (options.render == "html")
            {
                options.uri = PlayerSettings.libraryPath + @"\" + options.uri;
                RenderLayout.MediaNodeList.Add(
                new MediaOption(
                    options.layoutId,
                    options.regionId,
                    options.width,
                    options.height,
                    options.top,
                    options.left,
                    options.mediaId,
                    options.render,
                    options.type,
                    options.uri,
                    options.duration,
                    options.loop,
                    options.mute,
                    options.direction,
                    options.text,
                    options.documentTemplate,
                    options.copyrightNotice,
                    options.javaScript,
                    options.updateInterval,
                    options.scrollSpeed,
                    options.modeid,
                    options.audio,
                    options.transIn,
                    options.transInDuration,
                    options.transInDirection,
                    options.transOut,
                    options.transOutDuration,
                    options.transOutDirection,
                    options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                );
            }
            else
            {
                switch (options.type)
                {
                    case "image":
                        options.uri = PlayerSettings.libraryPath + @"\" + options.uri;
                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );
                        break;

                    case "powerpoint":
                        options.uri = PlayerSettings.libraryPath + @"\" + options.uri;
                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );
                        break;

                    case "video":
                        options.uri = PlayerSettings.libraryPath + options.uri;

                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );

                        break;

                    case "localvideo":
                        //options.uri = Uri.UnescapeDataString(options.uri);
                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );

                        break;

                    case "audio":
                        options.uri = PlayerSettings.libraryPath + options.uri;
                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );

                        break;

                    case "datasetview":
                    case "embedded":
                    case "ticker":
                    case "text":
                    case "webpage":
                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );
                        break;

                    case "flash":
                        options.uri = PlayerSettings.libraryPath + options.uri;
                        RenderLayout.MediaNodeList.Add(
                        new MediaOption(
                            options.layoutId,
                            options.regionId,
                            options.width,
                            options.height,
                            options.top,
                            options.left,
                            options.mediaId,
                            options.render,
                            options.type,
                            options.uri,
                            options.duration,
                            options.loop,
                            options.mute,
                            options.direction,
                            options.text,
                            options.documentTemplate,
                            options.copyrightNotice,
                            options.javaScript,
                            options.updateInterval,
                            options.scrollSpeed,
                            options.modeid,
                            options.audio,
                            options.transIn,
                            options.transInDuration,
                            options.transInDirection,
                            options.transOut,
                            options.transOutDuration,
                            options.transOutDirection,
                            options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                        );
                        break;

                    case "shellcommand":
                        RenderLayout.MediaNodeList.Add(
                            new MediaOption(
                                options.layoutId,
                                options.regionId,
                                options.width,
                                options.height,
                                options.top,
                                options.left,
                                options.mediaId,
                                options.render,
                                options.type,
                                options.uri,
                                options.duration,
                                options.loop,
                                options.mute,
                                options.direction,
                                options.text,
                                options.documentTemplate,
                                options.copyrightNotice,
                                options.javaScript,
                                options.updateInterval,
                                options.scrollSpeed,
                                options.modeid,
                                options.audio,
                                options.transIn,
                                options.transInDuration,
                                options.transInDirection,
                                options.transOut,
                                options.transOutDuration,
                                options.transOutDirection,
                                options.launchThroughCmd, options.scName, options.terminateCommand, options.useTaskkill, options.windowsCommand)
                            );
                        break;

                    default:
                        throw new InvalidOperationException("Not a valid media node type: " + options.type);
                }
            }
        }
    }
}
