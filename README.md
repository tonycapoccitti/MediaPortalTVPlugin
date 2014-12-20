<strong>An MB3 Live TV Plugin for Media Portal V1</strong>

This has been tested with MB3 Server Version 3.0.5451.540 and MP Version 1.8.0. You may have to wait for the next release of MB3 before using this plugin.

<p>This has a prerequisite of installing MPExtended on the box that hosts MediaPortal. Both v0.5.4 and v0.6 are supported - however running v0.5.4 does not allow recordings to be deleted. However v0.6 does, so download and install from here:</p>

<ul>
	<li>v0.5.4 -&nbsp;<a href="http://forum.team-mediaportal.com/threads/updated-14-jul-mpextended-webmediaportal-0-5-4.114106/">http://forum.team-mediaportal.com/threads/updated-14-jul-mpextended-webmediaportal-0-5-4.114106/</a></li>
	<li>v0.6 -&nbsp;<a href="http://forum.team-mediaportal.com/threads/mpextended-0-6-beta-release.125677/">http://forum.team-mediaportal.com/threads/mpextended-0-6-beta-release.125677/</a></li>
</ul>

<p>MPExtended allows for testing of itself, please check that it is working correctly by opening the configuration (from the system tray icon) going to Troubleshooting and clicking the links displayed. You should see a browser with something like &quot;{&quot;ApiVersion&quot;:4,&quot;HasConnectionToTVServer&quot;:true,&quot;ServiceVersion&quot;:&quot;0.5.4&quot;}&quot; displayed (depending on the version installed). We are only interested (currently) in the TV and Streaming services.<br />
&nbsp;<br />
<strong>Installation</strong></p>

<ul>
	<li>Find the Media Portal Live TV Plugin in the plugin catalog and install</li>
	<li>Restart MB3, and navigate to settings</li>
	<li>Go to Settings > Plugins and then to the configuration for Media Portal TV Plugin</li>
	<li>Configure the plugin, adding any authentication information and test the connection by clicking "Save and Test Connection.</li>
	<li>Once you enter the correct connection settings you will be able to select a Channel Group and a Streaming Profile. For the Streaming Profile Direct is recommended as this deffers transcoding to MB3</li>
	<li>Click on LiveTV and Refresh Guide Data</li>
	<li>Switch to the MB3 client UI and you shoud be good to go</li>
</ul>

<p><strong>Known Limitations</strong></p>

An MB3 Live TV Plugin for Media Portal V1 (1.8.0)

This version has been tested with MB3 Server Version 3.0.5451.540 and MP Version 1.8.0. You may have to wait for the next release of MB3 before using this plugin.

<ul>
	<li>MPExtended v0.5.4 does not allow deleting of recordings - please use v0.6 Beta as described above</li>
	<li>MPExtended doesn&#39;t currently support reading the genre mappings. I am working on an update to a fork of v0.6 in order to supply these values. Then the program meta data will be populated (IsMovie etc)</li>
	<li>Picking individual days for a series schedule (if more than one day is specifed and it is not Monday - Friday or Saturday - Sunday, we opt for recording everyday).&nbsp;<em>This is not supported by MP1</em></li>
	<li>Recording Only New Programs -&nbsp;<em>This is not supported by MP1</em></li>
</ul>

<p><strong>Feedback</strong><br />
&nbsp;<br />
I would appreciate any feedback by posting to this topic. Please raise any issues here:&nbsp;<a href="https://github.com/ChubbyArse/MediaPortalTVPlugin/issues">https://github.com/ChubbyArse/MediaPortalTVPlugin/issues</a>&nbsp;if you don&#39;t have a GitHub login, post against this topic and I&#39;ll add them to the issues list.<br />
<br />
TV Guide<br />
&nbsp;<br />
<img alt="xYlBB.jpg" src="http://snag.gy/xYlBB.jpg" style="border: 0px solid rgb(0, 0, 0) !important;" /><br />
&nbsp;<br />
Recordings<br />
&nbsp;<br />
<img alt="d41ns.jpg" src="http://snag.gy/d41ns.jpg" style="border: 0px solid rgb(0, 0, 0) !important;" /><br />
&nbsp;<br />
Channels<br />
&nbsp;<br />
<img alt="Ofq4J.jpg" src="http://snag.gy/Ofq4J.jpg" style="border: 0px solid rgb(0, 0, 0) !important;" /><br />
&nbsp;<br />
Series Schedule<br />
&nbsp;<br />
<img alt="x0lwd.jpg" src="http://snag.gy/x0lwd.jpg" style="border: 0px solid rgb(0, 0, 0) !important;" /></p>
