
SmartPlugMonitor v0.3
=====================

Author: ChrisDeadman

Tray-based power monitoring software for smart plugs.

## Feature list
* Supports TP-LINK HS1XX Series
* Supports Wattage,Voltage,Current monitoring
* Displays sensor information in system tray

## Usage
* Click on notification icon allows configuration
* Simply input ip-address and choose which sensors to show

## Supported Operating Systems
* Linux (tested on Ubuntu 18.04)
* Windows 7
* Windows 10

TP-LINK HS1XX protocol reverse-engineering credits go to [George Georgovassilis and Thomas Baust](https://blog.georgovassilis.com/2016/05/07/controlling-the-tp-link-hs100-wi-fi-smart-plug/)  
Application icon credits go to [tango.freedesktop.org](http://tango.freedesktop.org)

Release notes
=======================

### SmartPlugMonitor v0.3
* TP-LINK HS110: add support for HW version 2.0
* TP-LINK HS110: send message length in first 4 bytes instead of just zeroes

### SmartPlugMonitor v0.2
* tray icon tooltip now shows sensor name
* improve tray icon visibility
* now also supports linux (using gtk+ backend)

### SmartPlugMonitor v0.1
* Initial release
