// ==UserScript==
// @name        AddTicket
// @namespace   RisqueServer
// @include     https://risque.itap.purdue.edu/Tickets/Data/TicketDetail.aspx*
// @version     1
// @grant       none
// ==/UserScript==

var _enableButton;
var _addButton;
var _connectionButton;

var _dropDownMenu;
var _btnGroup;

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
}
function createConnectionButton() {
    var actionsDropdown = document.getElementById("contentMain_A1");
    var btnGroup = actionsDropdown.parentNode;
    //create Button
    var listItem = document.createElement("LI");
    var Aobj = document.createElement("A");
    Aobj.appendChild(document.createTextNode("Connect"));
    Aobj.onclick = connectClick;
    listItem.appendChild(Aobj);
    //listItem.style.backgroundColor = "#ffcfbf";
    _dropDownMenu.appendChild(listItem);
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
    }
    else {
        //clicked cancel
    }
    //document.cookie = "username=John Doe; expires=Thu, 18 Dec 2013 12:00:00 UTC; path=/";
}

function addClick() {
    console.log("Clicked Add button");
}

function connectClick() {
    console.log("Clicked connect button");
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
        console.log("key: " + keyValue[0]);
        console.log("value: " + keyValue[1]);
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