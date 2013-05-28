$(document).on("click", ".alert .close", function (e) {
    vm.importAlert(null);
});

// Check for the various File API support.
if (window.File && window.FileReader && window.FileList && window.Blob) {
    // Great success! All the File APIs are supported.
} else {
    alert('The File APIs are not fully supported in this browser.');
}

$('#uploadSvgButton').click(function (e) {
    $(this).button('loading');
    var url = "plans/importsvg?apikey=bda11d91-7ade-4da1-855d-24adfe39d174";
    $.ajax({
        url: url,
        data: vm.importSvg(),
        type: "POST",
        success: function (data, status, xhr) {
            $("#uploadSvgButton").button('reset');
            console.log('Import OK');
            vm.importSvg(null);
            ko.mapping.fromJS(data, {}, vm.importContainer);
            insertIds(vm.importContainer(), vm.selectedPlanId());
            document.getElementById('importFiles').innerHTML = '';
            $('#importModal').modal();
        },
        error: function (xhr, status, error) {
            $("#uploadSvgButton").button('reset');
            console.log('Import error');
            vm.importAlert("Невозможно импортировать файл - " + error);            
        },
    });
});

function insertIds(items, parentId) {
    ko.utils.arrayForEach(items, function (element) {
        element.Id(minId--);
        element.ParentId(parentId);
        if (!(typeof element.Children === "undefined"))
            insertIds(element.Children(), element.Id());
    });
}

$('#importSvgButton').click(function (e) {
    $(this).button('loading');
    ko.utils.arrayForEach(vm.importContainer(), function (element) {
        vm.selectedPlan().Children.push(element);
        $("#importSvgButton").button('reset');
        $('#importModal').modal('hide');
    });
});

function handleFileSelect(evt) {
    evt.stopPropagation();
    evt.preventDefault();    
    if (evt.dataTransfer.files.length != 1) {
        vm.importAlert('Выберите только один файл формата SVG');
        document.getElementById('importFiles').innerHTML = '';
        return;
    }
    var output = [];
    // files is a FileList of File objects. List some properties.
    var f = evt.dataTransfer.files[0];
    output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
        f.size, ' bytes, last modified: ',
        f.lastModifiedDate ? f.lastModifiedDate.toLocaleDateString() : 'n/a',
        '</li>');
    document.getElementById('importFiles').innerHTML = '<ul>' + output.join('') + '</ul>';
    var reader = new FileReader();
    reader.onload = (function (theFile) {
        return function (e) {
            vm.importSvg(e.target.result);
            vm.importAlert(null);
        };
    })(f);
    reader.readAsText(f);    
}

function handleDragOver(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    evt.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
}

// Setup the dnd listeners.
var dropZone = document.getElementById('drop_zone');
dropZone.addEventListener('dragover', handleDragOver, false);
dropZone.addEventListener('drop', handleFileSelect, false);