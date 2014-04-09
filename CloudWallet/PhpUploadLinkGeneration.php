<?php


$from = $_GET["from"]; // sender
$to = $_GET["to"]; // sender  
$Random = $_GET['RandomInt'];

$date = new DateTime();

$TimeStamp = $date->getTimestamp();
$ip = $_SERVER['REMOTE_ADDR'];


$UniqueStamp = sha1(md5($TimeStamp.$ip.$Random)); //creates an md5,sh1 timestamp

$File_Location = "";  
/*
if (strncasecmp(substr($_FILES["file"]["name"], -4), '.exe') === 0) {
    // is .exe
}*/

$allowedExts = array("wlt","cw");
$temp = explode(".", $_FILES["file"]["name"]);
$extension = end($temp);

if(($_FILES["file"]["size"] < 20098900)
&& in_array($extension, $allowedExts))
  {
  if ($_FILES["file"]["error"] > 0)
    {
    echo "Return Code: " . $_FILES["file"]["error"] . "<br>";
    }
  else
    {
  
    if (file_exists("upload/" . $_FILES["file"]["name"]))
      {
     "already exists.";
      }
    else
      {
      move_uploaded_file($_FILES["file"]["tmp_name"],
      "upload/" . $_FILES["file"]["name"].'reflection'.$UniqueStamp.'.CloudWallet');
      
    echo  $File_Location  = "http://fusionservers.x10.mx/CloudWallet"."/upload/" . $_FILES["file"]["name"].'reflection'.$UniqueStamp.'.CloudWallet';
 
$headers = array(
$headers .= 'Content-Type: text/html; charset="iso-8859-1"',
$headers .= 'MIME-Version: 1.0',
$headers .= 'From:  Fusion servers <fusionse@absolut.x10hosting.com>',
$headers .= 'To: Wisdom O <'.$to.'>' ,
$headers .= 'Reply-To: Developers <fusionse@absolut.x10hosting.com>',
$headers .= 'X-Mailer: PHP/5.2.5'

);
$headers = implode("\r\n", $headers);
$message = "Hello <b><font color=\"Green\">$to</font></b>, a friend of yours has sent you a file<br>";
$message .= "using CloudWallet. This file has been encrypted<br>,";
$message .= "so ask your friend: $from for the decryption key.<br><br>";


//This is the php message sent, it will me editted and futurised when webserver is running.
$message .= "To download your encrypted File: BTW this is a temp message, i will change it soon<br>";

$message .= "Visit <a href=\"$File_Location\">Cloud Wallet</a><br><br>";

$message .= "OR<br>";

$message .= "Goto <a href=\"http://fusionservers.com/wallet\">http://fusionservers.com/wallet</a><br><br>";

$message .= "AND<br><br>";

$message .= "enter the code: ". $_FILES["file"]["name"]."reflection".$UniqueStamp."<br><br><br>";


$message .= "#Request decryption key from: $From<br>";
$message .= "#This message has been sent to a via Cloud Wallet server.<br>";
$message .= "#This is a one-way messaging system, so please do not replay.";

mail( "$to", "subject", $message, "$headers"); 
      }
    }
  }
else
  {
  echo "Invalid file";

  }


?>