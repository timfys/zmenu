var model;
var deviceIdToEdit;
//list of models 
const models =
    [
        "Epson TM-T88VI"
    ];
//list of connections
const  connections =
    ["IP", "Bluetooth", "USB", "COM"];
$(document).ready(function () {
    let submenu = document.getElementById("sidebar-menu").querySelectorAll(".submenu");
    for (let i = 0; i < submenu.length; i++) {
        if (submenu[i].innerHTML.includes("Settings") && submenu[i].querySelector("ul").style.cssText.includes("display: none;")) {
            submenu[i].querySelector("a").click();
        }
    }
    $('#AddPrinter').on('hidden.bs.modal', function () {
        document.getElementById("PortPrinterModal").value = '9100';
    });
});
//add device to view, and call request function
function AddDevice(event) {
    var type = event.target.id.toString();
    var typeV;
    var name;
    var modelV;
    var modelDevStr;
    var connection;
    var address;
    var actions;
    var line;
    var index = findTempId();
    var iConnection;
    var buttonEdit;
    var buttonDelete;
    if (type == "cash") {
        typeV = document.createElement("th");
        typeV.appendChild(document.createTextNode("Cash Drawer"));
        name = document.createElement("th");
        name.appendChild(document.createTextNode(document.getElementById('NameCashDrawer').value.toString()));
        modelV = document.createElement("th");
        modelV.appendChild(document.createTextNode(""/*models[document.getElementById('selectCashDrawerModel').selectedIndex].toString()*/));
        modelDevStr = "";// models[document.getElementById('selectCashDrawerModel').selectedIndex].toString();
        connection = document.createElement("th");
        connection.appendChild(document.createTextNode(""/*connections[document.getElementById('selectCashDrawerConnection').selectedIndex].toString()*/));
        iConnection = document.getElementById('selectCashDrawerConnection').selectedIndex;
        port = document.createElement("th");
        port.appendChild(document.createTextNode(""/*document.getElementById('PortCashDrawerModal').value.toString()*/));
        address = document.createElement("th");
        address.appendChild(document.createTextNode(document.getElementById('AddressCashDrawerModal').value.toString()));
        actions = document.createElement("th");
        buttonEdit = document.createElement("button");
        buttonDelete = document.createElement("button");
        buttonEdit.classList.add("table");
        buttonDelete.classList.add("table");
        buttonEdit.innerHTML = "<i class='fa fa-pencil ' style='z - index: -2; position: inherit'></i>";
        buttonDelete.innerHTML = "<i class='fa fa-times ' style='z - index: -2; position: inherit'></i>";
        //buttonEdit.classList.add(["btn"]);
        buttonEdit.className = "btn";
        buttonDelete.className = "btn m-1";
        actions.appendChild(buttonEdit);
        actions.appendChild(buttonDelete);
        name.setAttribute("name", "name");
        typeV.setAttribute("name", "type");
        modelV.setAttribute("name", "model");
        connection.setAttribute("name", "connection");
        address.setAttribute("name", "address");
        port.setAttribute("name", "port");
        line = document.createElement("TR");
        line.id = index;
        line.appendChild(name);
        line.appendChild(typeV);
        line.appendChild(modelV);
        line.appendChild(connection);
        line.appendChild(address);
        line.appendChild(actions);
        buttonEdit.addEventListener('click', OnRowClick, true);
        buttonDelete.addEventListener('click', DeleteDevice, true);
        document.getElementById("tableDevices").getElementsByTagName("TBODY")[0].appendChild(line);

    }
    else if (type == "printer") {
        typeV = document.createElement("th");
        typeV.appendChild(document.createTextNode("Printer")); 
        name = document.createElement("th");
        name.appendChild(document.createTextNode(document.getElementById('NamePrinterModal').value.toString()));
        modelV = document.createElement("th");
        modelV.appendChild(document.createTextNode(models[document.getElementById('selectModelPrinter').selectedIndex].toString()));
        modelDevStr = models[document.getElementById('selectModelPrinter').selectedIndex].toString();
        connection = document.createElement("th");
        connection.appendChild(document.createTextNode(connections[document.getElementById('selectConnectionPrinter').selectedIndex].toString()));
        iConnection = document.getElementById('selectConnectionPrinter').selectedIndex;
        address = document.createElement("th");
        address.appendChild(document.createTextNode(document.getElementById('AddressPrinterModal').value.toString()));
        port = document.createElement("th");
        port.appendChild(document.createTextNode(document.getElementById('PortPrinterModal').value.toString()));
        actions = document.createElement("th");
        buttonEdit = document.createElement("button");
        buttonDelete = document.createElement("button");
        //buttonEdit.classList.add(["btn"]);
        buttonEdit.className = "btn";
        buttonDelete.className = "btn m-1";
        buttonEdit.innerHTML = "<i class='fa fa-pencil ' style='z - index: -2; position: inherit'></i>";
        buttonDelete.innerHTML = "<i class='fa fa-times ' style='z - index: -2; position: inherit'></i>";
        actions.appendChild(buttonEdit);
        actions.appendChild(buttonDelete);
        name.setAttribute("name","name");
        typeV.setAttribute("name","type");
        modelV.setAttribute("name","model");
        connection.setAttribute("name","connection");
        address.setAttribute("name","address");
        port.setAttribute("name","port");
        line = document.createElement('TR');
        line.id = index;
        line.appendChild(name);
        line.appendChild(typeV);
        line.appendChild(modelV);
        line.appendChild(connection);
        line.appendChild(address);
        line.appendChild(actions);
        buttonEdit.addEventListener('click', OnRowClick, true);
        buttonDelete.addEventListener('click', DeleteDevice, true);
        document.getElementById("tableDevices").getElementsByTagName("TBODY")[0].appendChild(line);
    }
    var modelId = model.push(
        {
            bDeviceID: index,
            type:(type=="printer")?1:2,
            name: name.textContent,
            model: modelDevStr,
            connection: iConnection,
            address: address.textContent,
            port: port.textContent
        })-1;
    DeviceUpdReq(
        {
            bDeviceID: -1,
            type: (type == "printer") ? 1 : 2,
            name: name.textContent,
            model: modelDevStr,
            connection: iConnection,
            address: address.textContent,
            port: port.textContent,
            isdeleted: 0

        },
        modelId);

}
//event handler on row click
function OnRowClick(event) {
    
    let currNode = event.target;
    let id = currNode.id;
    while (id == "") {
        currNode = currNode.parentNode;
        id = currNode.id;
    }
    $('#EditDevice').modal('show');
    var currentDevice;
    for(var i=0;i<model.length;i++)
    {
        if(model[i].bDeviceID==id)
            currentDevice=model[i];
    }
    document.getElementById('editType').selectedIndex =       currentDevice.type-1;
    document.getElementById('editName').value =               currentDevice.name;
    document.getElementById('editModel').selectedIndex =      currentDevice.model;
    document.getElementById('editConnection').selectedIndex = currentDevice.connection;
    document.getElementById('editAddress').value =            currentDevice.address;
    document.getElementById('editPort').value =               currentDevice.port;
    deviceIdToEdit = id;
    typeChange();
}
function EditDevice(event)
{
    var currentDevice;
    currentDevice=
    {
        bDeviceID:deviceIdToEdit,
        type : document.getElementById('editType').selectedIndex+1,
        name : document.getElementById('editName').value,
        model : models[document.getElementById('editModel').selectedIndex ],
        connection : document.getElementById('editConnection').selectedIndex ,
        address : document.getElementById('editAddress').value,
        port : document.getElementById('editPort').value
    }
    for(var i=0;i<model.length;i++)
    {
        if(model[i].bDeviceID==deviceIdToEdit)
            {
                model[i].type = currentDevice.type;
                model[i].name = currentDevice.name;
                model[i].model = currentDevice.model;
                model[i].connection = currentDevice.connection;
                model[i].address = currentDevice.address;
                model[i].port = currentDevice.port;
            }
    }
    document.getElementById(deviceIdToEdit).querySelector('[name = "name"]').firstChild.textContent = currentDevice.name;
    document.getElementById(deviceIdToEdit).querySelector('[name = "type"]').firstChild.textContent= currentDevice.type == 1?"Printer":"Cash drawer";
    document.getElementById(deviceIdToEdit).querySelector('[name = "model"]').firstChild.textContent = currentDevice.model;
    document.getElementById(deviceIdToEdit).querySelector('[name = "connection"]').firstChild.textContent = connections[currentDevice.connection];
    document.getElementById(deviceIdToEdit).querySelector('[name = "address"]').firstChild.textContent = currentDevice.address;
    document.getElementById(deviceIdToEdit).querySelector('[name = "port"]').firstChild.textContent = currentDevice.port;
    DeviceUpdReq(currentDevice,-1);
}
function DeleteDevice(event) {
    let currNode = event.target;
    let DevId = currNode.id;
    while (DevId == "") {
        currNode = currNode.parentNode;
        DevId = currNode.id;
    }
    document.getElementById(DevId).remove();
    var currentElem;
    for (var i = 0; i < model.length; i++) {
        if (model[i].bDeviceID == DevId ) {
            currentElem = model[i];
            model.splice(i, 1);
        }
    }
    DeviceUpdReq(
        {
            bDeviceID: currentElem.bDeviceID,
            type: currentElem.type,
            name: currentElem.name,
            model: currentElem.model,
            connection: currentElem.connection,
            address: currentElem.address,
            port: currentElem.port,
            isdeleted: 1

        },-1);
}
function findTempId() {
    var i = -1;
    for (var j = 0; j < model.length; j++) {
        if (model[j].bDeviceID == i) {
            i--;
            j = 0;
            continue;
        }
    }
    return i;
}
function typeChange()
{
    modelChange();
    let modal = document.getElementById("EditDevice");
    let currType = document.getElementById("editType").selectedIndex;
    if (currType === 1) {
        modal.querySelector("[name = 'Model']").style.display = "none";
        modal.querySelector("[name = 'Connection']").style.display = "none";
        modal.querySelector("[name = 'Port']").style.display = "none";
        modal.querySelector("[name = 'Adress']").style.display = "none";

    }
    else
    {
        modal.querySelector("[name = 'Model']").style.display = "block";
        modal.querySelector("[name = 'Connection']").style.display = "block";
        modal.querySelector("[name = 'Port']").style.display = "block";
        modal.querySelector("[name = 'Adress']").style.display = "block";
    }
}
function modelChange()
{
    let modal = document.getElementById("EditDevice");
    if (modal.querySelector("[name = 'Model']").querySelector('select').selectedIndex == "0" && modal.querySelector("[name = 'Port']").querySelector('input').value == "")
        modal.querySelector("[name = 'Port']").querySelector('input').value = '9100';
}
