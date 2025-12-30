function createCatalogTable(data) {
    tbl = document.getElementById("catalogTable");
    tbl.classList.add("table", "table-striped", "custom-table");

    var columnHeaderData = ["Supplier Name", "Barcode </br> Product Name", "Price", "Available </br> Last Ordered", ""];
    addColumnHeader(tbl, columnHeaderData);
    addCatalogData(tbl, data);
}

function createCartTable(data) {
    tbl = document.getElementById("cartTable");
    tbl.classList.add("table", "table-striped", "custom-table");

    var columnHeaderData = ["Barcode </br> Product Name", "Cost </br> Ordered", "Remark to supplier", ""];
    addColumnHeader(tbl, columnHeaderData);
    addCartData(tbl, data);

    //edit link
    $(".linkEditCart").click( function(e) {
        e.preventDefault();
        
        var id = this.id.split("_");
        $('#EditCartProductId').val(id[1]);
        $('#EditCartPurchase_ProductId').val(id[2]);

        var RemarkToSupplier = Enumerable.From(cartData).
            Where(x => x.ProductId == id[1] && x.Purchase_ProductId == id[2]).
            Select(x => x.Remark_To_Supplier).
            FirstOrDefault();

        $('#EditCartRemarkToSupplier').val(RemarkToSupplier);
        $('#editCartModal').modal();

        return false; 
    });

    //remove link
    $(".linkRemoveCart").click( function(e) {
        e.preventDefault(); 
        
        RemoveFromCart(this.id.split("_")[1], this.id.split("_")[2]);
        return false; 
    });
    
}

function cartTableReload(data) {
    tbl = document.getElementById("cartTable");
    tbl.innerHTML = "";

    createCartTable(data);
}

function catalogTableReload(data) {
    tbl = document.getElementById("catalogTable");
    tbl.innerHTML = "";

    createCatalogTable(data);
}

function addColumnHeader(table, data) {
    if (!data)
        return;

    var thead = document.createElement("thead");
    data.forEach(function (item, i, data) {
        addTh(item, thead);
    });
    table.appendChild(thead);
}

function addTh(item, thead) {
    var th = document.createElement("th");
    th.innerHTML = item;
    thead.appendChild(th);
}

function addCatalogData(table, data) {
    if (!data)
        return;

    var tbody = document.createElement("tbody");
    data.forEach(function (item, i, data) {
        var tr = document.createElement("tr");
        addCell(item.Company, tr);
        addCell("<a href='' class='linkid' data-toggle='modal' data-target='#barCodeModal' onclick='$(\"#barCodeValue\").text(\"" + item.Barcode + "\")'>" + item.Barcode + "</a></br>"+item.ProductName, tr);
        addCell(item.Symbol + item.Price, tr);
        var cls = parseInt(item.QtyInStock) && item.QtyInStock <= 0 ? "num-negative-bold" : "num-positive-bold";
        addCell("<span class = '" + cls + "'>" + (parseInt(item.QtyInStock) ? (item.QtyInStock - cartData.filter(p => p.Purchase_ProductId == item.Purchase_ProductId && p.ProductId == item.ProductId).length) : "&#8734;") + "</span></br> <div title='" + item.LastPurchased + "'>" + item.LastPurchased + "</div>", tr);
        addCell((!parseInt(item.QtyInStock) || (item.QtyInStock - cartData.filter(p => p.Purchase_ProductId == item.Purchase_ProductId && p.ProductId == item.ProductId).length) > 0 ? "<button class='btn btn-dark' id='bt_" + item.ProductId + "_" + item.Purchase_ProductId + "' onClick='AddToCart(this.id.split(\"_\")[1], this.id.split(\"_\")[2])'>Add</button>" : "N/A"), tr);
        
        tbody.appendChild(tr);
    });

    table.appendChild(tbody);
}

function addCartData(table, data) {
    if (!data)
        return;

    var tbody = document.createElement("tbody");
    var company = "";

    var supplierData = Enumerable.From(data)
        .GroupBy(x => x.Company, null,
            function (key, g) {
                var result = {
                    company: key,
                    total: g.Sum("$.Price")
                };
                return result;
            }).ToArray();

    supplierData.forEach(function (item, i, supplierData) {
        // add supplier group
        var trSup = document.createElement("tr");
        addCell("<h3>" + item.company+"</h3>", trSup);
        addCell("<h3>$"+item.total+"</h3>", trSup);
        addCell("", trSup);
        addCell("", trSup);
        tbody.appendChild(trSup);


        //add item
        var productData = Enumerable.From(data)
            .Where(x => x.Company == item.company)
            .GroupBy(x => x.ProductId + "_" + x.Purchase_ProductId, null,
                function (key, g) {
                    var result = {
                        id: key,
                        product: "<a href='' class='linkid' data-toggle='modal' data-target='#barCodeModal' onclick='$(\"#barCodeValue\").text(\"" + g.source[0].Barcode + "\")'>" + g.source[0].Barcode + "</a></br>" + g.source[0].ProductName,
                        costOrdered: "$" + g.source[0].Price + "</br>" + g.Count(),
                        remarkToSupplier: g.source[0].Remark_To_Supplier
                    };
                return result;
            }).ToArray();


        productData.forEach(function (item, i, productData) {
            tr = document.createElement("tr");
            addCell(item.product, tr);
            addCell(item.costOrdered, tr);
            addCell(item.remarkToSupplier, tr);
            addCell("<a href='#' class='linkEditCart' id='ed_" + item.id +"'>Edit</a> </br> <a href='#' class='linkRemoveCart' id='rm_" + item.id +"'>Remove</a>", tr);
        
            tbody.appendChild(tr);
        });
        
    });

    table.appendChild(tbody);

}

function addCell(item, tr) {
    var td = document.createElement('td');

    td.innerHTML = item;

    tr.appendChild(td);
}
