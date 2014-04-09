<?php
$URL = "";
if(isSet($_GET["Link"]))
{
$URL = $_GET["Link"];
}
else if(isSet($_POST["Link"])){
$URL = $_POST["Link"];
}

echo $URL;


if (file_exists("upload/" .$URL.'CloudWallet'))
      {
     
	 //Continue// This script will be completed when webserver is online

}
else
{
echo '<br<p>Sorry this file is no longer in our database</p>';'
}

?>