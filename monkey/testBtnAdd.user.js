// ==UserScript==
// @name        testBtnAdd
// @namespace   everettr
// @include     https://risque.itap.purdue.edu/Tickets/Data/TicketDetail.aspx*
// @version     1
// @grant       none
// ==/UserScript==

var _button = null;
var _sock = null;

function testGet() {
    console.log("Testing get");
    //must allow network.websocket.allowInsecureFromHTTPS
    //var Socket = new WebSocket("ws://glitch.tcom.purdue.edu:8181");
    var Socket = new WebSocket("ws://localhost:8181");
    
    Socket.onopen = function () {
        console.log("Succesfully connected to WebSocket");
        console.log("Attempting to send 'Hello'");
        Socket.send("Hello");
    }
    Socket.onmessage = function (evt) {
        console.log("Received data: " + evt.data);
    }
    Socket.onclose = function () {
        console.log("closed socket");
    }
}

function createSocket() {
    var Socket;
    try {
        Socket = new WebSocket("ws://localhost:8181");
        //Socket = new WebSocket("ws://glitch.tcom.purdue.edu:8181");
        Socket.onopen = function () {
            console.log("Succesfully connected to WebSocket");
        }
        Socket.onmessage = function (evt) {
            console.log("Received data: " + evt.data);
        }
        Socket.onclose = function () {
            console.log("closed socket");
        }
        _sock = Socket;
        return true;
    }
    catch (Exception) {
        console.log(Exception.message);
        return false;
    }
}

function addTicket(ticketData) {
    var Request = {
        method: "addTicket",
        params: ticketData
    }
    if (_sock != null) {
        //console.log(JSON.stringify(Request));
        _sock.send(JSON.stringify(Request));
    }
    else {
        throw "Socket is null!";
    }
}

function buttonClick() {
    if (_sock != null) {
        var ticketInfo = tryParse();        //Returns the Object, not the string
        if (ticketInfo != null) {
            try {
                addTicket(ticketInfo);
            }
            catch (Exception) {
                console.log("buttonClick() failed with Exception: " + Exception.message);
            }
        }
        else {
            alert("Failed to parse Ticket Info!");
        }
    }
    else {
        createSocket();

    }
}

//CHANGED BUTTON ACTION
function buttonTest() {
    alert("Clicked button!");
    testGet();
}
function tryParse() {
    var ticketTable = document.getElementById("contentMain_grdItems");
    var itemPopulate = new Array(ticketTable.rows.length - 1); //ignore header row
    console.log("itemPopulate length: " + itemPopulate.length + ", ticketTable.rows: " + ticketTable.rows.length);


    //get ID, Due By, and Internal Status (ignore if not Active, probably should throw error)
    var _ticketID = document.getElementById("contentMain_lblTicketID").innerText;
    var _dueBy = document.getElementById("contentMain_lblTicketDue").innerText;
    var internalStatus = document.getElementById("contentMain_lblTicketInternalStatus").innerText;

    if (internalStatus == "Active") {
        //Ticket Items
        for (var i = 0; i < ticketTable.rows.length; i++) {
            if (i == 0) continue; //skip header row
            var _actionType, _picID, _provider;
            var _currSpeed, _currVoiceVlan;
            var _currVlans = new Array();
            var _newSpeed, _newVlan, _newVoiceVlan;
            //var newServices = new Array();
            /*
              "Modify"
            */
            _actionType = ticketTable.rows[i].cells[0].innerText;
            /*
              "PIC ID:  STEW-B14-D
              Patch: stew-17-pp24-02-20 (back)
              Provider: stew-17-c3750ep-01:03-Gi3/0/27"           //Shouldn't be any spaces on the right side of provider
            */
            var portInfoElement = ticketTable.rows[i].cells[1].innerText.split('\n');
            _picID = portInfoElement[0].split(" ")[1].trim().toString();
            var ugly_providersplit = portInfoElement[2].split(":");
            _provider = (ugly_providersplit[1] + ":" + ugly_providersplit[2]).trim().toString();
            console.log("_actionType: " + _actionType + ", _picID: " + _picID + ", _provider: " + _provider);
            /*
              "Current Speed: 
                 10/100T-SW-A
              Current VLAN(s): 
                 128.210.217.000/24 Public Subnet (217)
              Current VoIP VLAN: 
                010.010.118.000/23-STEW-VoIP-17_Voice (2978)
              New Speed: 
                10/100/1000T-SW-A
      
              New VLAN:
                010.162.017.000/24-STEW-CSDS-Supported_Computers_1 (1211)
      
              New VoIP VLAN: 
                010.010.118.000/23-STEW-VoIP-17_Voice (2978)
              New Services: None
              "
            */
            //use string.trim() to get rid of that bullshit padding
            var settingsElement = ticketTable.rows[i].cells[2].innerText.split('\n');
            var lastHeader;
            for (var j = 0; i < settingsElement.length; j++) {
                if (settingsElement[j].indexOf("Current") !== -1 && settingsElement[j].indexOf("Speed") !== -1) {
                    lastHeader = "currSpeed";
                    continue;
                }
                else if (settingsElement[j].indexOf("Current") !== -1 && settingsElement[j].indexOf("VLAN(s)") !== -1) {
                    lastHeader = "currVlan";
                    continue;
                }
                else if (settingsElement[j].indexOf("Current") !== -1 && settingsElement[j].indexOf("VoIP") !== -1) {
                    lastHeader = "currVoice";
                    continue;
                }
                else if (settingsElement[j].indexOf("New") !== -1 && settingsElement[j].indexOf("Speed") !== -1) {
                    lastHeader = "newSpeed";
                    continue;
                }
                else if (settingsElement[j].indexOf("New") !== -1 && settingsElement[j].indexOf("VoIP") !== -1) {
                    lastHeader = "newVoice";
                    continue;
                }
                else if (settingsElement[j].indexOf("New") !== -1 && settingsElement[j].indexOf("VLAN") !== -1) {
                    lastHeader = "newVlan";
                    continue;
                }
                else if (settingsElement[j].indexOf("New") !== -1 && settingsElement[j].indexOf("Services") !== -1) {
                    console.log("Services not supported");
                    break;
                }
                if (settingsElement[j].trim() == "") {
                    continue;
                }
                switch (lastHeader) {
                    case "currSpeed":
                        _currSpeed = settingsElement[j].trim();
                        break;
                    case "currVlan":
                        _currVlans.push(settingsElement[j].trim());
                        break;
                    case "currVoice":
                        _currVoiceVlan = settingsElement[j].trim();
                        break;
                    case "newSpeed":
                        _newSpeed = settingsElement[j].trim();
                        break;
                    case "newVlan":
                        _newVlan = settingsElement[j].trim();
                        break;
                    case "newVoice":
                        _newVoiceVlan = settingsElement[j].trim();
                        break;
                }
            }
            console.log("currSpeed: " + _currSpeed + ", currVlan: " + _currVlans.toString() + ", currVoice: " + _currVoiceVlan);
            console.log("newSpeed: " + _newSpeed + ", newVlan: " + _newVlan + ", newVoiceVlan: " + _newVoiceVlan);
            //console.log("newServices: " + newServices.toString());
            var _portInfo = { actionType: _actionType, picID: _picID, provider: _provider };
            var _settings = {
                currSpeed: _currSpeed,
                currVlans: _currVlans,
                currVoiceVlan: _currVoiceVlan,
                newSpeed: _newSpeed,
                newVlan: _newVlan,
                newVoiceVlan: _newVoiceVlan
            };
            var Action = { portInfo: _portInfo, settings: _settings };
            itemPopulate[i - 1] = Action;
        }
        var TicketObj = {
            ticketID: _ticketID,
            dueBy: _dueBy,
            Actions: itemPopulate
        };
        return TicketObj;
    }
    else {
        alert("Internal Status is not Active, will not continue");
        return null;
    }

}

function createButton() {
    var actionsDropdown = document.getElementById("contentMain_A1");
    var btnGroup = actionsDropdown.parentNode;
    var controls = btnGroup.parentNode;
    //var buttonHtml = "<button>Click me</button>";
    var button = document.createElement("BUTTON");
    button.appendChild(document.createTextNode("Click me"));
    controls.appendChild(button);
    button.type = "button";
    //button.onclick=buttonTest;
    button.onclick = buttonClick;  //CHANGED BUTTON ACTION
    _button = button;
    createSocket();
}

//https://github.com/dsrdakota/onDomReady
//DON'T TOUCH PLS
if (document.ondomready == undefined) {
    document.ondomready = {};
    document.ondomready = null;
} else {
    document.ondomready = document.ondomready;
}

var oldonload = document.onload;
var isLaunched = 0;

document.onload = function () {
    if (oldonload !== null) {
        oldonload.call();
    }
};
document.addEventListener("DOMContentLoaded", function onDom(event) {
    var olddomready = document.ondomready;
    if (olddomready !== null) {
        if (isLaunched == 0) {
            olddomready.call(this, event);
            isLaunched == 1;
            //We now launched the mapped DOMContentLoaded Event
        } else {
            //We already launched DOMContentLoaded Event
        }
    }
}, false);

//Injection Point
document.ondomready = function (event) {
    console.log("DOM Ready, firing createButton()");
    createButton();
};