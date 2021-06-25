/*jslint browser: true, devel: true, white: true, eqeq: true, plusplus: true, sloppy: true, vars: true*/
/*global $ */

/*************** General ***************/

var updateOutput = function (e) {
    var list = e.length ? e : $(e.target),
        output = list.data('output');
    if (window.JSON) {
        if (output) {
            output.val(window.JSON.stringify(list.nestable('serialize')));
        }
    } else {
        alert('JSON browser support required for this page.');
    }
};

var nestableList = $(".dd.nestable > .dd-list");
/***************************************/

/*************** Delete ***************/

var deleteFromMenuHelper = function (target) {
    if (target.data('new') === 1) {
        // if it's not yet saved in the database, just remove it from DOM
        target.fadeOut(function () {
            target.remove();
            updateOutput($('.dd.nestable').data('output', $('#json-output')));
        });
    } else {
        // otherwise hide and mark it for deletion
        target.appendTo(nestableList); // if children, move to the top level
        target.data('deleted', '1');
        target.fadeOut();
    }
};

var deleteFromMenu = function () {
    var targetId = $(this).data('owner-id');
    var target = $('[data-id="' + targetId + '"]');

    var result = confirm("Delete '" + target[0].innerText + "' and all its subitems ?");
    if (!result) {
        return;
    }
    // Remove children (if any)
    target.find("li").each(function () {
        deleteFromMenuHelper($(this));
    });
    // Remove parent
    deleteFromMenuHelper(target);
    // update JSON
    updateOutput($('.dd.nestable').data('output', $('#json-output')));
};

/***************************************/

/*************** Edit ***************/

var menuEditor = $("#menu-editor");
var editButton = $("#editButton");
var editInputName = $("#editInputName");
var editInputPrefix = $("#editInputPrefix");
//var editInputSlug = $("#editInputSlug");
var editInputAction = $("#DynamicMenuTextEdit");
var editInputLink = $("#editInputLink");
//var currentEditName = $("#currentEditName");
var currentEditName = $("#editInputName");


var oldValue = "";
var newValue = "";

var oldValueLink = "";
var newValueLink = "";

var oldValuePrefix = "";
var newValuePrefix = "";



// Prepares and shows the Edit Form
var prepareEdit = function () {
    var targetId = $(this).data('owner-id');
    var target = $('[data-id="' + targetId + '"]');
    var actiontype = target.data("type");
    console.log("Action Type - " + actiontype);
    if (actiontype == 2) {
        $("#menuPrefix1").show();
        $("#linkId1").hide();
    }
    else if (actiontype == 3) {
        $("#menuPrefix1").hide();
        $("#linkId1").show();
    }
    else {
        $("#menuPrefix1").hide();
        $("#linkId1").hide();
    }
    editInputName.val(target.data("text"));
    editInputPrefix.val(target.data("prefix"));
    editInputAction.val(target.data("type"));
    editInputLink.val(target.data("href"));
    currentEditName.html(target.data("name"));

    oldValue = target.data("text");
    oldValueLink = target.data("href");
    oldValuePrefix = target.data("prefix");

    editButton.data("owner-id", target.data("id"));
    console.log("[INFO] Editing Menu Item " + editButton.data("owner-id"));
    menuEditor.fadeIn();
};

// Edits the Menu item and hides the Edit Form
var editMenuItem = function () {
    var targetId = $(this).data('owner-id');
    var target = $('[data-id="' + targetId + '"]');
    var newName = editInputName.val();
    var newAction = editInputAction.val();
    var newLink = editInputLink.val();
    var newPrefix = editInputPrefix.val();
    target.data("text", newName);
    target.data("name", newName);
    target.data("type", newAction);
    target.data("href", newLink);
    target.data("prefix", newPrefix);
    target.find("> .dd-handle").html(newName);
    menuEditor.fadeOut();
    // update JSON

    newValue = oldValue + ":" + newName + newValue + " |";
    newValuePrefix = oldValuePrefix + ":" + newPrefix + newValuePrefix + " |";
    newValueLink = oldValueLink + ":" + newLink + newValueLink + " |";

    $("#html-name").val(newValue);
    $("#html-prefix").val(newValuePrefix);
    $("#html-link").val(newValueLink);

    updateOutput($('.dd.nestable').data('output', $('#json-output')));
    alert('Menu Edited Successfully');
    $("#editInputName").val('');
    $("#addInputPrefix").val('');
    $("#addInputLink").val('');
    $("#DynamicMenuText").val('1');
};

/***************************************/


/*************** Add ***************/

//var newIdCount = 1;
var newIdCount = $("#MaxId").val();
//alert(newIdCount);

var addToMenu = function () {
    var newName = $("#addInputName").val();
    var newPrefix = $("#addInputPrefix").val();
    var newSlug = $("#addInputSlug").val();
    var newId = newIdCount === 0 ? 1 : newIdCount;
    var newType = $("#DynamicMenuText").val();
    var hrefLink = $("#addInputLink").val();

    if (newType == 1) {
        hrefLink = "";
    }
    else if (newType == 2) {
        hrefLink = window.location.protocol + "//" + window.location.host + "/MSPS/DetailsView?id=" + newId;
    }

    nestableList.append(
        '<li class="dd-item" ' +
        'data-id="' + newId + '" ' +
        'data-text="' + newName + '" ' +
        'data-name="' + newName + '" ' +
        'data-prefix="' + newPrefix + '" ' +
        'data-type="' + newType + '" ' +
        'data-href="' + hrefLink + '" ' +
        'data-slug="' + newSlug + '" ' +
        'data-new="1"  data-deleted="0"> <div class="dd-handle">' + newName + '</div> ' +
        '<span class="button-delete btn btn-default btn-xs pull-right" data-owner-id="' + newId + '"> ' +
        '<i class="fa fa-times-circle-o" aria-hidden="true"></i></span>' +
        '<span class="button-edit btn btn-default btn-xs pull-right" data-owner-id="' + newId + '">' +
        '<i class="fa fa-pencil" aria-hidden="true"></i> </span> </li>'
    );
    newIdCount++;
    // update JSON
    updateOutput($('.dd.nestable').data('output', $('#json-output')));
    alert('Menu Created Successfully');

    $("#addInputName").val('');
    $("#editInputPrefix").val('');
    $("#editInputLink").val('');
    $("#editInputAction").val('1');

    console.log("[INFO] New ID - " + newIdCount);
    var ss = nestableList;
    // set events
    $(".dd.nestable .button-delete").on("click", deleteFromMenu);
    $(".dd.nestable .button-edit").on("click", prepareEdit);
};

/***************************************/
$(function () {

    // output initial serialised data
    updateOutput($('.dd.nestable').data('output', $('#json-output')));
    // set onclick events
    editButton.on("click", editMenuItem);
    $(".dd.nestable .button-delete").on("click", deleteFromMenu);
    $(".dd.nestable .button-edit").on("click", prepareEdit);
    $("#menu-editor").submit(function (e) {
        e.preventDefault();
    });
    $("#menu-add").submit(function (e) {
        e.preventDefault();
        addToMenu();
    });
});