<?php

namespace Custom\Libraries;

interface Log
{
    public function debug($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function error($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);

    public function notice($summary, $source = null, array $xRefArray = null, $message = null, $timeElapsed = null);
}
