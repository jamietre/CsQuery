<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CsQuery.WebFormsApp._Default" %>
<%@ Register TagPrefix="myApp" TagName="WebUserControl1" Src="WebUserControl1.ascx" %>

<!DOCTYPE html>
<html lang="en-US" xmlns="http://www.w3.org/1999/xhtml" dir="ltr">
<head>
	<title>CsQuery WebForms Integration Demo</title>
	<meta http-equiv="Content-type" content="text/html; charset=utf-8" />
	<link rel="stylesheet" href="styles/style.css" type="text/css" media="all" />
	<!--[if IE 6]>
		<link rel="stylesheet" href="styles/ie6.css" type="text/css" media="all" />
	<![endif]-->
</head>
<body>

<script type="text/javascript">
    function notImplemented(title) {
        alert("Sorry! '"+title+"' is not implemented.");
    }
</script>
<!-- Header -->
<div id="header">

	<div class="shell">
		
		<div id="logo-holder">
			<h1 id="logo">CsQuery WebForms Integration</h1>
			<p id="quote">Never Use A Webcontrol Again!</p>
		</div>
	
		<div id="navigation">
			<ul>
			    <li><a href="#" class="active" title="Home"><span>Home</span></a></li>
			    <li><a href="#" title="About Us"><span>About Us</span></a></li>
			    <li><a href="#" title="Services"><span>Services</span></a></li>
			    <li><a href="#" title="Support"><span>Support</span></a></li>
			    <li><a href="#" title="Partners"><span>Partners</span></a></li>
			    <li><a href="#" title="Contact"><span>Contact</span></a></li>
			</ul>
		</div>
		
		<div class="cl">&nbsp;</div>
		
	</div>
</div>
	<!-- End Header -->
	
	<!-- Featured Content -->
	<div id="featured-content">
		<div class="shell">
			<h2>welcome</h2>
			<p><b>A basic web site showing how to integrate CsQuery into a webforms application for both Page and UserControl entites.</b>
            Take a look at the markup and codebehind for <code>Default.aspx</code> and <code>WebUserControl1.aspx</code> to see how it's done.
            
            </p>
		</div>
	</div>
	<!-- End Featured Content -->
	
<!-- Main -->
<div id="main">
	<div class="shell">
		<!-- Content Cols -->
		
		
		<!-- Main Content -->
       

		<div id="main-content">
			<div class="post">
				<h3>A user control with content generated using CsQuery</h3>
				 <myApp:WebUserControl1 ID="WebUserControl1" runat="server" TableColumns="6" TableRows="3" />
			</div>
		</div>
		<!-- End Main Content -->
		
		<!-- Sidebar -->
		
		<div id="sidebar">
			<div class="post">
				<h3>What's Happening?</h3>
                
				<p>The HTML for the table to the left was generated dynamically on the server using values passed as
                UserControl parameters to define the number of rows and columns, and populate the cells.</p>
			</div>
		</div>
		
		<!-- End Sidebar -->
		
		<div class="cl">&nbsp;</div>
		
	</div>	
</div>
<!-- End Main -->	

<!-- Footer -->
<div id="footer">
	<div class="shell">
		<div id="footer-links" class="left">
			<p>
			    <a href="#" title="Home">home</a>
			    <a href="#" title="About Us">about us</a>
			    <a href="#" title="Services">services</a>
			    <a href="#" title="Solutions">solutions</a>
			    <a href="#" title="Support">support</a>
			    <a href="#" title="Partners">partners</a>
			    <a href="#" title="Contact">contact</a>
			</p>
		</div>
	</div>
</div>
<!-- End Footer -->

</body>
</html>
