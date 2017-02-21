// ==UserScript==
// @name        AddTicket
// @namespace   RisqueServer
// @include     https://risque.itap.purdue.edu/Tickets/Data/TicketDetail.aspx*
// @version     1
// @grant       none
// ==/UserScript==

//Buttons
var _enableButton;
var _addButton;
var _connectionButton;

//Risque Globals
var _dropDownMenu;
var _btnGroup;

//Socket Specific
var _sock;                                              //The Socket itself
var _serverAddress = "ws://localhost:8181";             //The Address the socket connects to on start up
var _recieveCallback = new Function("message");         //The callback the socket Object uses to return to calling functions
var _isConnected = false;
var _isConnecting = false;

function createButton() {
    var actionsDropdown = document.getElementById("contentMain_A1");
    var btnGroup = actionsDropdown.parentNode;
    //create Button
    var listItem = document.createElement("LI");
    var Aobj = document.createElement("A");
    Aobj.appendChild(document.createTextNode("Testy"));
    Aobj.onclick = function () {
        window.alert("Clicked!");
    }
    listItem.appendChild(Aobj);
    listItem.style.backgroundColor = "#ffcfbf";
    //button.onclick = buttonClick;  //CHANGED BUTTON ACTION
    //_button = button;
    //button.onclick=buttonTest;
    //Attach button
    for (var i= 0; i < btnGroup.childNodes.length; i++) {
        var child = btnGroup.childNodes[i];
        console.log(child);
        if (child.className == "dropdown-menu") {
            console.log("Found!");
            child.appendChild(listItem);
            break;
        }
    }
    /*btnGroup.childNodes[3].appendChild(button);*/
    //createSocket();
}
function createAddButton() {
    var actionsDropdown = document.getElementById("contentMain_A1");
    var btnGroup = actionsDropdown.parentNode;
    //create Button
    var listItem = document.createElement("LI");
    var Aobj = document.createElement("A");
    Aobj.appendChild(document.createTextNode("Add Ticket"));
    Aobj.onclick = addClick;
    listItem.appendChild(Aobj);
    //listItem.style.backgroundColor = "#ffcfbf";
    _dropDownMenu.appendChild(listItem);
    _addButton = Aobj;
}
function createConnectionButton() {
    var actionsDropdown = document.getElementById("contentMain_A1");
    var btnGroup = actionsDropdown.parentNode;
    //create Button
    var listItem = document.createElement("LI");
    var Aobj = document.createElement("A");
    Aobj.appendChild(document.createTextNode("Connecting"));
    //Aobj.onclick = connectClick;
    listItem.appendChild(Aobj);
    //listItem.style.backgroundColor = "#ffcfbf";
    _dropDownMenu.appendChild(listItem);
    _connectionButton = Aobj;
}
function SockRecievedMessage(evt) {
    console.log("Recieved: " + evt.data);
    if (evt.data == "Hello") {
        return;
    }
    _recieveCallback(evt.data);
}
function SockClose() {
    console.log("Closed Socket");
    _connectionButton.onclick = connectClick;
    _connectionButton.innerText = "Connect";
    _connectionButton.parentNode.backgroundColor = "red";
    _isConnected = false;
    _isConnecting = false;
    //Update connection button
}
function SockOpen() {
    console.log("Created WebSocket");
    _isConnected = true;
    _isConnecting = false;
    _connectionButton.onclick = null;
    _connectionButton.innerText = "Connected";
    _connectionButton.parentNode.backgroundColor = "green";
    _recieveCallback = null;
}
function SockError(evt) {
    console.log("Sock received an error: " + evt.data);
}
function createSocket() {
    var Socket;
    try {
        _isConnecting = true;
        _connectionButton.innerText = "Connecting";
        Socket = new WebSocket(_serverAddress);
        Socket.onopen = SockOpen;
        Socket.onmessage = SockRecievedMessage;
        Socket.onclose = SockClose;
        Socket.onerror = SockError;
        _sock = Socket;
        _isConnected = true;
        return true;
    }
    catch (Exception) {
        //failed to connect
        console.log(Exception.message);
        _isConnecting = false;
        _isConnected = false;
        _connectionButton.onclick = connectClick;
        _connectionButton.innerText = "Not Connected";
        _connectionButton.parentNode.backgroundColor = "red";
        return false;
    }
}

function enableClick() {
    //set cookie=true or create cookie
    if (window.confirm("Do you wish to enable the RisqueServer extra features? (ALPHA)")) {
        console.log("Clicked yes");
        document.cookie = "RisqueServerEnabled=true; expires=Monday, 1 May 2017 12:00:00 UTC; path=/";
        //remove enable button
        _enableButton.parentNode.removeChild(_enableButton);
        //create other buttons
        createAddButton();
        createConnectionButton();
        createSocket();
    }
    else {
        //clicked cancel
    }
    //document.cookie = "username=John Doe; expires=Thu, 18 Dec 2013 12:00:00 UTC; path=/";
}
function containsTicket(successCallback, failCallback) {
    if (_sock != null) {
        var ticketID = document.getElementById("contentMain_lblTicketID").innerText;
        var requestObject = { method: "doesTicketExist", params: { id: ticketID } };
        var sockMessage = "Content-Type: json" + '\n' + JSON.stringify(requestObject) + '\n';
        _sock.send(sockMessage);
        _recieveCallback = function (message) {
            _recieveCallback = null;
            console.log("Recieved callback CONTAINSTICKET");
            var indexOfFirstNewLine = message.indexOf('\n');
            var newMessage = message.substring(indexOfFirstNewLine, message.length);
            var parsed = JSON.parse(newMessage);
            console.log("Successfully parsed object: " + parsed);
            if (parsed.success == true) {
                console.log("Success");
                console.log("Exists: " + parsed.exists);
                if (parsed.exists) {
                    successCallback();
                }
                else {
                    failCallback();
                }
            }
            else {
                console.log("Doesn't exist");
                console.log(parsed.failureReason);
                throw "Method failed to execute properly";
            }
        }
    }
    throw "Socket is null!";
}
/*function buttonTest() {
    alert("Clicked button!");
    testGet();
}*/
function AddTicket(Ticket, successCallback) {
    if (_sock != null) {
        var Request = {
            method: "addTicketInfo",
            params: Ticket
        }
        _recieveCallback = function (message) {
            _recieveCallback = null;
            console.log("Recieved callback ADDTICKET");
            var indexOfFirstNewLine = message.indexOf('\n');
            var newMessage = message.substring(indexOfFirstNewLine, message.length);
            var parsedObj = JSON.parse(newMessage);
            if (parsedObj.success) {
                successCallback();
            }
            else {
                throw "Method failed";
            }
        };
        _sock.send("Content-Type: json" + '\n' + JSON.stringify(Request) + '\n');
    }
    else {
        console.log("Socket is closed");
        throw "Socket is null, must be closed or something?";
    }
}
function GetTicketData() {
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
            if (_provider === ":undefined") _provider = null;
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

function addClick() {
    console.log("Clicked Add button");
    /*if (!containsTicket()) {
        console.log("Can add ticket");
    }*/
    containsTicket(function () {
        console.log("Asynchronous callback, return true function");
        window.alert("Ticket already exists in the system!");
    }, function () {
        console.log("Asynchronous callback, return false function");
        //can add ticket since it doesn't exist
        var TicketData = GetTicketData();
        if (window.confirm("Add Ticket: " + TicketData.ticketID + " to Server?")) {
            //add ticket
            AddTicket(TicketData, function () {
                //success
                console.log("Added Ticket");
            });
        }
        else {
            console.log("User declined adding ticket");
        }
    })
}

function connectClick() {
    console.log("Clicked connect button");
    if (!_isConnecting && !_isConnected) {
        createSocket();
    }
}

function createEnableButton() {
    var actionsDropdown = document.getElementById("contentMain_A1");
    var btnGroup = actionsDropdown.parentNode;
    //create Button
    var listItem = document.createElement("LI");
    listItem.id = "RisqueServerEnableBtn";
    var Aobj = document.createElement("A");
    Aobj.appendChild(document.createTextNode("Enable RisqueServer"));
    Aobj.onclick = enableClick;
    listItem.appendChild(Aobj);
    listItem.style.backgroundColor = "#ffcfbf";
    //button.onclick = buttonClick;  //CHANGED BUTTON ACTION
    //_button = button;
    //button.onclick=buttonTest;
    //Attach button
    _dropDownMenu.appendChild(listItem);
    _enableButton = listItem;
}

function isEnabled() {
    //if not enabled, create enable button
    //if enabled, create other buttons
    //console.log("isEnabled");
    var cookies = document.cookie.split(";");
    var hasCookie = false;
    //console.log("Cookies length: " + cookies.length);
    for (var i = 0; i < cookies.length; i++) {
        var keyValue = cookies[i].split("=");
        //console.log("key: " + keyValue[0]);
        //console.log("value: " + keyValue[1]);
        if (keyValue[0].trim() === "RisqueServerEnabled") {
            if (keyValue[1] === "true") {
                hasCookie = true;
            }
        }
    }
    if (hasCookie) {
        //create other buttons
        console.log("Has cookie");
        createAddButton();
        createConnectionButton();
        createSocket();
    }
    else {
        //create enable button
        console.log("Has no cookie");
        createEnableButton();
    }
}
function getGlobals() {
    //get dropdown and btnGroup
    console.log("getting globals");
    var actionsDropdown = document.getElementById("contentMain_A1");
    _btnGroup = actionsDropdown.parentNode;
    for (var i = 0; i < _btnGroup.childNodes.length; i++) {
        var child = _btnGroup.childNodes[i];
        if (child.className == "dropdown-menu") {
            _dropDownMenu = child;
            break;
        }
    }
    console.log("gotem");
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
    getGlobals();
    isEnabled();
};