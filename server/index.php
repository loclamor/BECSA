<?php

require_once './conf/init.php';
?>
<!DOCTYPE html>
<html>
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title><?php echo Site::getInstance()->getTitle(); ?></title>
    </head>
    <body>
        <?php
        echo Site::getInstance()->getContent();
        ?>
    </body>
</html>
