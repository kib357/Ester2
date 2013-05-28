var paper;
var tool = 'tool_selection';
var minId;
var elementTypes1 = [{ name: 'Геометрия', value: '20' },
                    { name: 'Надпись', value: '30' },
                    { name: 'Одиночная лампа', value: '58' },
                    { name: 'Кондиционер', value: '60' },
                    { name: 'Датчик температуры', value: '51' },
                    { name: 'Датчик освещенности', value: '52' },
                    { name: 'Датчик протечки', value: '53' },
                    { name: 'Отопление', value: '59' }];
var elementTypes2 = [{ name: 'Этажный план', value: '1' },
                    { name: 'План ИТП', value: '2' },
                    { name: 'Схема ПВУ', value: '3' },
                    { name: 'Схема электроснабжения', value: '4' },
                    { name: 'Схема медиа', value: '5' },
                    { name: 'Группа объектов', value: '15' },
                    { name: 'Помещение', value: '50' },
                    { name: 'Группа ламп', value: '56' }];

function getTypeName(id) {
    for (i = 0; i < elementTypes1.length; i++)
        if (elementTypes1[i].value == id) return elementTypes1[i].name;
    for (i = 0; i < elementTypes2.length; i++)
        if (elementTypes2[i].value == id) return elementTypes2[i].name;
    return null;
}

function isSensor(typeId) {
    return typeId == 8 || typeId == 9 || typeId == 10 || typeId == 11 || typeId == 12 || typeId == 13;
}

function isContainer(typeId) {
    for (i = 0; i < elementTypes2.length; i++)
        if (elementTypes2[i].value == typeId) return true;
    return false;
}

function getSensorPath(typeId) {
    if (typeId == 58) return lampPath;
    if (typeId == 60) return conditionerPath;
    if (typeId == 51) return temperaturePath;
    if (typeId == 52) return lightSensorPath;
    if (typeId == 53) return waterLeakPath;
    if (typeId == 59) return heaterPath;
}

//--------------------------------------Begin viewmodel----------------------------------------

function AppViewModel() {
    var self = this;
    //recursively create a flat array
    var addChildren = function (array, result) {
        array = ko.utils.unwrapObservable(array);
        if (array) {
            for (var i = 0, j = array.length; i < j; i++) {
                result.push(array[i]);
                if (!(typeof array[i].Children === "undefined"))
                    addChildren(array[i].Children, result);
            }
        }
    };

    // Plans
    self.plans = ko.observableArray();
    self.trackPlanHistory = ko.observable(false);
    self.plansHistory = ko.observableArray([]);
    self.plansHistoryPosition = ko.observable(1);
    //self.plansHistoryPosition.subscribe(function(n) { console.log('pos: ' + n); });
    self.plansHistoryPusher = ko.computed(function () {
        var snapShot = ko.toJS(self.plans());
        if (self.trackPlanHistory() == true) {
            if (self.plansHistoryPosition() > 1) {
                self.plansHistory.splice(0, self.plansHistoryPosition() - 1);
                self.plansHistoryPosition(1);
            }
            self.plansHistory.splice(0, 0, snapShot);
            //console.log(self.plansHistory().length);
        }
        if (self.trackPlanHistory() == null)
            self.trackPlanHistory(true);
    });
    self.plansHistoryCanUndo = ko.computed(function () {
        if (self.plansHistory().length > self.plansHistoryPosition()) return true;
        return false;
    });
    self.plansHistoryCanRedo = ko.computed(function () {
        if (self.plansHistoryPosition() > 1) return true;
        return false;
    });
    self.selectedPlanId = ko.observable();
    self.selectedPlan = ko.computed(function () {
        var result = null;
        if (self.selectedPlanId() != null)
            result = ko.utils.arrayFirst(self.plans(), function (item) {
                return item.Id() == self.selectedPlanId();
            });
        return result;
    });

    // Elements
    self.elements = ko.computed(function () {
        var res = [];
        var e = ko.utils.arrayFirst(this.plans(), function(item) {
            if (!(typeof item === "undefined") && item != null) return false;
            return item.Id() == self.selectedPlanId();
        });
        if (e != null && e.Children() != null)
            res = e.Children();
        return res;
    }, this);
    self.selectedElementId = ko.observable();
    self.selectedElement = ko.computed(function () {
        var result = null;
        if (self.selectedElementId() != null) {
            result = ko.utils.arrayFirst(self.flatElements(), function (item) {
                return item.Id() == self.selectedElementId();
            });
        }
        return result;
    });
    self.selectedElement.subscribe(function (newElement) {
        clearSelection();
        if (newElement == null) return;
        $('#element_' + newElement.Id()).addClass("text-error");
        loadProperties(newElement);
    });
    self.flatSelectedElement = ko.computed(function () {
        var result = [];
        if (self.selectedElement() != null) {
            result.push(self.selectedElement());
            if (!(typeof self.selectedElement().Children === "undefined"))
                addChildren(self.selectedElement().Children(), result);
        }
        return result;
    });
    self.selectedElementGraphic = ko.computed(function () {
        if (self.selectedElement() == null) return null;
        var res = paper.set();
        ko.utils.arrayForEach(vm.flatSelectedElement(), function (e) {
            var childGraphic = paper.getById(e.Id());
            if (childGraphic != null)
                res.push(childGraphic);
        });
        return res;
    });
    self.selectedElementGraphic.subscribe(function () { showSelectionBox(); });

    // A flattened versions of elements
    self.flatElements = ko.computed(function () {
        var result = [];
        if (self.selectedPlan() != null)
            addChildren(self.selectedPlan().Children(), result);
        return result;
    });
    self.flatElements.subscribe(function (newValue) {
        RefreshCanvas(newValue);
    });
    self.properties = ko.observableArray([]);


    // Import elements
    self.importAlert = ko.observable();
    self.importSvg = ko.observable();
    self.importContainer = ko.observableArray([]);
    self.importFlatElements = ko.computed(function () {
        var result = [];
        if (self.importContainer() != null)
            addChildren(self.importContainer(), result);
        return result;
    });
    self.importFlatElements.subscribe(function (newValue) {
        $("#svgImport").empty();
        var p = Raphael("svgImport", 530, 400);
        for (i = 0; i < newValue.length; i++)
            DrawSvg(newValue[i], p);
    });

    //Element types
    self.primitiveTypes = ko.observableArray(elementTypes1);
    self.containerTypes = ko.observableArray(elementTypes2);
    self.elementTypeId = ko.observable();
    
    //fakeDataLookup
    self.fakeRangeMin = ko.observable(0);
    self.fakeRangeMax = ko.observable(10);
    self.fakeInterval = ko.observable(5);
    self.fakeIsWritable = ko.observable(false);
    self.fakeValue = ko.observable(10);
    
    //Zoom and pan
    self.zoom = ko.observable(1);
    self.svgWidth = ko.observable(null);
    self.svgHeight = ko.observable(null);
    self.svgCanvasWidth = ko.observable(null);
    self.svgCanvasHeight = ko.observable(null);
    self.scaleX = ko.computed(function (){
        
    });
    self.scaleY = ko.observable();
    self.zoom.subscribe(function (newValue) {
        if (paper == null || 
            self.svgWidth() == null || self.svgHeight() == null || 
            self.svgCanvasWidth() == null || self.svgCanvasHeight() == null) return;
        //paper.setViewBox(0, 0, self.svgWidth() / self.scaleX() * newValue, self.svgHeight() / self.scaleY() * newValue);
    });
}
var vm = new AppViewModel();
ko.applyBindings(vm);

function loadProperties(newElement) {
    if (newElement.Properties() == null)
        if (newElement.Id() >= 0)
            $.get('plans/properties/' + newElement.Id() + '?apikey=bda11d91-7ade-4da1-855d-24adfe39d174', function (data) {
                ko.mapping.fromJS(data, {}, vm.properties);
            });
        else
            $.get('plans/typeproperties/' + newElement.TypeId() + '?apikey=bda11d91-7ade-4da1-855d-24adfe39d174', function (data) {
                ko.mapping.fromJS(data, {}, vm.properties);
            });
    else {
        ko.mapping.fromJS(ko.toJS(newElement.Properties), {}, vm.properties);
    }
}

function getElement(id) {
    var result = ko.utils.arrayFirst(vm.flatElements(), function (item) {
        return item.Id() == id;
    });
    if (result == null && vm.selectedPlanId() == id)
        result = vm.selectedPlan();
    return result;
};

function Undo() {
    if (vm.plansHistoryCanUndo()) {
        vm.trackPlanHistory(false);
        ko.mapping.fromJS(vm.plansHistory()[vm.plansHistoryPosition()], {}, vm.plans);
        vm.plansHistoryPosition(vm.plansHistoryPosition() + 1);
        vm.trackPlanHistory(null);
    }
}

function Redo() {
    if (vm.plansHistoryCanRedo()) {
        vm.trackPlanHistory(false);
        vm.plansHistoryPosition(vm.plansHistoryPosition() - 1);
        ko.mapping.fromJS(vm.plansHistory()[vm.plansHistoryPosition() - 1], {}, vm.plans);        
        vm.trackPlanHistory(null);
    }
}

//--------------------------------------Events----------------------------------------

$(document).ready(function () {
    if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
        $(document).on('keypress', function (e) {
            handleKeyboardEvents(e);
        });
    } else {
        $(document).on('keyup', function (e) {
            handleKeyboardEvents(e);
        });
    }
    initUI();
    RefreshPage();    
});

$(document).on("click", ".planLink", function () {
    $(".planLink.active").removeClass("active");
    $(this).addClass("active");
    vm.selectedPlanId($(this).children().attr("id"));
    return false;
});

$(document).on("click", ".elementLink", function () {
    vm.selectedElementId(this.id.replace('element_', ''));
    return false;
});

$('.toolbar-btn').on('click', function () {
    $(".toolbar-btn.btn-primary").removeClass("btn-primary").addClass("btn-inverse");
    $(this).removeClass("btn-inverse").addClass("btn-primary");
    if (tool != this.id) {
        tool = this.id;
        vm.selectedElementId(null);
    }
});

$("#savePlansButton").click(function () {
    $(this).button('loading');
    ko.utils.arrayForEach(vm.plans(), function (plan) {
        UploadPlanObject(plan);
    });
    $(this).button('reset');
    RefreshPage();
});

$("#addPlanButton").click(function () {
    var plan = createBaseObject("Новый план", 1, null);
    plan.Width(600);
    plan.Height(400);
    vm.plans.push(plan);
});

$("#deletePlanButton").click(function () {
    $("#deletePlanButton").button('loading');
    DeletePlan(vm.selectedPlan());
});

$("#saveElementButton").click(function () {
    ko.mapping.fromJS(ko.toJS(vm.properties), {}, vm.selectedElement().Properties);
    vm.selectedElementId(null);
});

$("#cancelElementChangesButton").click(function () {
    vm.selectedElementId(null);
});

$("#deleteElementButton").click(function () {
    $("#deleteElementButton").button('loading');
    DeleteElement(vm.selectedElement());
});

$("#changeElementTypeConfirmationButton").click(function () {
    vm.elementTypeId(vm.selectedElement().TypeId());
    $("#changeElementTypeModal").modal();
});

$("#changeElementTypeButton").click(function () {
    $(this).button('loading');
    if (vm.elementTypeId != null) {
        var typeId = vm.elementTypeId();
        vm.selectedElement().TypeId(typeId);
        vm.selectedElement().Type(getTypeName(typeId));
        $.get('plans/typeproperties/' + vm.selectedElement().TypeId() + '?apikey=bda11d91-7ade-4da1-855d-24adfe39d174', function (data) {
            vm.properties(data);
            vm.selectedElement().Properties(ko.toJS(vm.properties));
        });
        RefreshCanvas(vm.flatElements());
        showSelectionBox();
    }
    vm.elementTypeId(null);
    $(this).button('reset');
    $('#changeElementTypeModal').modal('hide');
});

$(document).on("click", ".propertyLookup", function () {
    $('#propertyIdHidden').val($(this).data('id'));
    $('#propertyTypeIdHidden').val($(this).data('typeid'));
    $('#propertyLookupModal').modal();
    return false;
});

$("#selectAddressButton").click(function() {
    var tabId = $('.lookup.active').attr('id');
    var propertyId = $('#propertyIdHidden').val();
    var propertyTypeId = $('#propertyTypeIdHidden').val();
    property = ko.utils.arrayFirst(vm.properties(), function(item) {
        return item.Id() == propertyId && item.TypeId() == propertyTypeId;
    });
    var res = '';
    if (tabId == "fakeTab") {
        if (vm.fakeIsWritable())
            res = "F;w(" + vm.fakeValue() + ")";
        else
            res = "F;d(" + vm.fakeRangeMin() + "," + vm.fakeRangeMax() + "," + vm.fakeInterval() + ")";
        property.Path(res);
    }
    if (tabId == "bacnetTab") {        
        property.Path(res);
    }
    $('#propertyLookupModal').modal('hide');
});

function clearSelection() {
    $(".elementLink.text-error").removeClass("text-error");
    hideSelectionBox();
    vm.properties([]);
}

//-------------------------------Work with paths---------------------------------//
var _selectionBox;

function graphicMouseDown(e) {
    var posx = e.pageX - $(document).scrollLeft() - $('#svgcanvas').offset().left;
    var posy = e.pageY - $(document).scrollTop() - $('#svgcanvas').offset().top;
    switch (tool) {
        case 'tool_selection':
            vm.selectedElementId(this.id);
            break;
        case 'tool_addTemperatureSensor':
            addNewSensor(10, this, posx, posy);
            break;
        case 'tool_addLightSensor':
            addNewSensor(11, this, posx, posy);
            break;
        case 'tool_addWaterLeakSensor':
            addNewSensor(12, this, posx, posy);
            break;
        case 'tool_addHeater':
            addNewSensor(13, this, posx, posy);
            break;
        case 'tool_addConditioner':
            addNewSensor(9, this, posx, posy);
            break;
        case 'tool_addLamp':
            addNewSensor(8, this, posx, posy);
            break;
        default:
            break;
    }
}

function addNewSensor(typeId, graphicItem, posx, posy) {
    var parent = getElement(graphicItem.id);
    if (parent.Type() == "Помещение") {
        var obj = createBaseObject('Новый датчик', typeId, parent.Id());
        obj.Left(posx);
        obj.Top(posy);
        parent.Children.push(obj);
    }
}

function hideSelectionBox() {
    if (_selectionBox != null) {
        _selectionBox.remove();
        _selectionBox = null;
    }
}

function showSelectionBox() {
    if (vm.selectedElementGraphic() == null) return;
    var box = vm.selectedElementGraphic().getBBox();
    hideSelectionBox();
    //_selectionBox = paper.rect(box["x"] - 1, box["y"] - 1, box["width"] + 2, box["height"] + 2)
    _selectionBox = paper.rect(box["x"], box["y"], box["width"], box["height"])
      .attr("stroke-dasharray", "--")
      .attr("stroke", "#FF2D00");
}

var pathMoved = false;
function pathDragMove(dx, dy, x, y, event) {
    if (tool != 'tool_selection') return;
    //if (dx > 1 || dy > 1) {
    hideSelectionBox();
    vm.selectedElementGraphic().transform("t" + (dx / vm.scaleX()) + "," + (dy / vm.scaleY()));
    pathMoved = true;
    //}
}

function pathDragEnd(event) {
    if (pathMoved == false) return;
    pathMoved = false;
    if (tool != 'tool_selection') return;
    removeTransform();
    showSelectionBox();
    //console.log("dragEnd");
}

function MoveElement(x, y) {
    hideSelectionBox();
    vm.selectedElementGraphic().transform("t" + x + "," + y);
    removeTransform();
    showSelectionBox();
}

function removeTransform() {
    vm.trackPlanHistory(false);
    vm.selectedElementGraphic().forEach(function (item) {
        //var transform = item.transform();
        var translate = getTranslate(item);
        if (translate == null) return;
        var element = getElement(item.id);
        if (element == null) return;
        switch (item.type) {
            case "rect":
                item.attr('x', item.attr('x') + translate[0]);
                item.attr('y', item.attr('y') + translate[1]);
                item.transform("");
                element.Left(item.attr('x'));
                element.Top(item.attr('y'));
                break;
            case "path":
                var pathData = Raphael.pathToRelative(item.attr("path"));
                pathData[0][1] = pathData[0][1] + translate[0];
                pathData[0][2] = pathData[0][2] + translate[1];
                item.transform("")
                    .attr("path", pathData);
                if (element.Path != null && typeof element.Path.Data === 'function') //если у элемента есть заданная геометрия
                    element.Path.Data(item.attr("path").toString());
                else {
                    element.Left(pathData[0][1]);
                    element.Top(pathData[0][2]);
                }
                break;
        }                
    });
    vm.trackPlanHistory(true);
}

function getTranslate(item) {
    var transform = item.transform();
    var res = null;
    for (i = 0; i < transform.length; i++) {
        if (transform[i][0] == 't')
            res = [transform[i][1], transform[i][2]];
    }
    return res;
}

$("#svgcanvas").on("click", function (event) {
    switch (tool) {
        case 'tool_selection':
            if (event.target.nodeName == "svg") vm.selectedElementId(null);
            break;
        default:
            break;
    }
});

//-------------------------------Ajax requests---------------------------------//

function UploadPlanObject(observableObj) {
    var obj = ko.toJS(observableObj);
    var url = "plans/" + (obj.Id && obj.Id >= 0 ? obj.Id : '') + "?apikey=bda11d91-7ade-4da1-855d-24adfe39d174";
    var jsonText = JSON.stringify(obj);
    $.ajax({
        url: url,
        data: jsonText,
        type: "POST",
        error: function (xhr, status, error) {
            alert(xhr.status + " .Error: " + xhr.statusText);
        },
        async: false
    });
}

function DeleteElement(obj) {
    if (obj.Id() < 0) {
        DeleteElement2(obj);
        return;
    }
    var url = "plans/" + obj.Id() + "?apikey=bda11d91-7ade-4da1-855d-24adfe39d174";
    $.ajax({
        url: url,
        type: "DELETE",
        success: function (data, status, xhr) {
            var parent = getElement(obj.ParentId());
            if (parent != null)
                parent.Children.remove(obj);
            else
                vm.selectedPlan().Children.remove(obj);
            vm.selectedElementId(null);
        },
        error: function (xhr, status, error) {
            alert(xhr.status + " .Error: " + xhr.statusText);
        },
        async: false
    });
    $('#deleteElementModal').modal('hide');
    $("#deleteElementButton").button('reset');
}

function DeleteElement2(obj) {
    if (obj.ParentId() != null) {
        var parent = getElement(obj.ParentId());
        if (parent != null)
            parent.Children.remove(obj);
        else
            vm.selectedPlan().Children.remove(obj);
        vm.selectedElementId(null);
    } else {
        vm.plans.remove(obj);
    }    
    $('#deleteElementModal').modal('hide');
    $("#deleteElementButton").button('reset');
    $('#deletePlanModal').modal('hide');
    $("#deletePlanButton").button('reset');
}

function DeletePlan(obj) {
    if (obj.Id() < 0) {
        DeleteElement2(obj);
        return;
    }
    vm.selectedElementId(null);
    var url = "plans/" + obj.Id() + "?apikey=bda11d91-7ade-4da1-855d-24adfe39d174";
    $.ajax({
        url: url,
        type: "DELETE",
        success: function (data, status, xhr) {
            var index = vm.plans.indexOf(obj) - 1;
            vm.plans.remove(obj);
            if (index >= 0 && index < vm.plans().length) {
                vm.selectedPlanId(vm.plans()[index].Id());
                $(".planLink.active").removeClass("active");
                $("#plan_" + vm.selectedPlanId()).addClass("active");
            }
        },
        error: function (xhr, status, error) {
            alert(xhr.status + " .Error: " + xhr.statusText);
        },
        async: false
    });
    $('#deletePlanModal').modal('hide');
    $("#deletePlanButton").button('reset');
}

//-------------------------------Utility---------------------------------//

function UnHide(element) {
    if ($(element).hasClass("icon-chevron-right")) {
        $(element).removeClass("icon-chevron-right").addClass("icon-chevron-down");
        element.parentNode.parentNode.parentNode.className = '';
    } else {
        $(element).removeClass("icon-chevron-down").addClass("icon-chevron-right");
        element.parentNode.parentNode.parentNode.className = 'cl';
    }
    return false;
}

function RefreshCanvas(elements) {
    $("#svgcanvas").empty();
    vm.svgCanvasWidth($('#svgcanvas').width());
    vm.svgCanvasHeight($('#svgcanvas').width());
    paper = Raphael("svgcanvas", vm.svgCanvasWidth(), vm.svgCanvasHeight());
    paper.setStart();
    if (elements != null)
        for (i = 0; i < elements.length; i++)
            DrawSvg(elements[i]);
    var s = paper.setFinish();
    var box = s.getBBox();
    vm.svgWidth(box["x2"]);
    vm.svgHeight(box["y2"]);
    vm.zoom(1.1);
    SetDragDrop();
    //paper.setViewBox(0, 0, vm.svgWidth() / vm.scaleX() * vm.zoom(), vm.svgHeight() / vm.scaleY() * vm.zoom());
    //paper.rect(box["x"], box["y"], box["width"], box["height"])
    //  .attr("stroke-dasharray", "--")
    //  .attr("stroke", "#FF2D00");
}

function createBaseObject(name, typeId, parentId) {
    var obj = {
        Id: ko.observable(minId--),
        Type: ko.observable(getTypeName(typeId)),
        TypeId: ko.observable(typeId),
        ParentId: ko.observable(parentId),
        Name: ko.observable(name),
        Description: ko.observable(),
        ZIndex: ko.observable(0),
        Left: ko.observable(0.0),
        Top: ko.observable(0.0),
        Width: ko.observable(0.0),
        Height: ko.observable(0.0),
        IsContainer: ko.observable(isContainer(typeId)),
        Children: ko.observableArray(),
        Properties: ko.observable(null)
    };
    return obj;
}

function DrawSvg(planObject, paperToDraw) {
    if (paperToDraw == null)
        paperToDraw = paper;
    if (planObject == null) return;
    if ((typeof planObject.Path === "object"))
        var geometry = planObject.Path;
    if (geometry != null) {
        var path = paperToDraw.path(geometry.Data());
        if (geometry.Fill)
            path.attr("fill", geometry.Fill());
        if (geometry.Stroke)
            path.attr("stroke", geometry.Stroke());
        if (geometry.Title)
            path.attr("title", planObject.Name());
        path.drag(pathDragMove, null, pathDragEnd);
        path.mousedown(graphicMouseDown);
        path.id = planObject.Id();
        if (planObject.TypeId() == 6)
            path.node.setAttribute("class", "room");
    }
    if (isSensor(planObject.TypeId())) {
        //var rect = paperToDraw.rect(planObject.Left(), planObject.Top(), 30, 30).attr("fill", '#346534');
        //rect.drag(pathDragMove, null, pathDragEnd);
        //rect.mousedown(pathClick);
        //rect.id = planObject.Id();
        var sensorPath = getSensorPath(planObject.TypeId());
        var path1 = paperToDraw.path(sensorPath[1]);
        var pathData = Raphael.pathToRelative(path1.attr("path"));
        pathData[0][1] = pathData[0][1] + planObject.Left();
        pathData[0][2] = pathData[0][2] + planObject.Top();
        path1.attr("path", pathData);
        path1.attr("fill", sensorPath[0]);
        path1.attr("stroke-width", 0);
        path1.drag(pathDragMove, null, pathDragEnd);
        path1.mousedown(graphicMouseDown);
        path1.id = planObject.Id();
    }
    //if (planObject.TypeId == 4) {
    //    SVG.append("text")
    //		.text(planObject.Name);
    //}
}

function RefreshPage() {
    vm.trackPlanHistory(false);
    var si = vm.selectedPlanId();
    vm.selectedElementId(null);
    vm.selectedPlanId(null);
    $.get('plans/?apikey=bda11d91-7ade-4da1-855d-24adfe39d174', function (data) {
        ko.mapping.fromJS(data, {}, vm.plans);
        if (vm.plans().length > 0) {
            $(".planLink.active").removeClass("active");
            var previousPlan = ko.utils.arrayFirst(vm.plans(), function (item) {
                return item.Id() == si;
            });
            if (previousPlan != null) {
                vm.selectedPlanId(si);
                $("#plan_" + vm.selectedPlanId()).addClass("active");
            } else if (vm.plans().length > 0) {
                vm.selectedPlanId(vm.plans()[0].Id());
                $("#plan_" + vm.selectedPlanId()).addClass("active");
            }
        }
        minId = -1;
        SetDragDrop();
        vm.trackPlanHistory(true);
    });
}

function SetDragDrop() {
    $(".element-drag").draggable({
        appendTo: "body",
        helper: "clone",
        //start: function (event, ui) {
        //    var element = $(this).parent().parent().parent().addClass("cl");
        //    $(this).parent().children('i').removeClass("icon-chevron-down").addClass("icon-chevron-right");
        //},
    });
    $(".element-drop").droppable({
        over: function (event, ui) {
            var newParent = getElement($(this).attr('id').replace('element_', ''));
            var element = getElement($(ui.draggable).attr('id').replace('element_', ''));
            if (canPlace(element, newParent))
                $(this).addClass('label label-success');
        },
        out: function (event, ui) {
            $(this).removeClass('label label-success');
        },
        drop: function (event, ui) {
            var newParent = getElement($(this).attr('id').replace('element_', ''));
            var element = getElement($(ui.draggable).attr('id').replace('element_', ''));
            if (canPlace(element, newParent)) {
                vm.trackPlanHistory(false);
                ui.draggable.remove();
                var oldParent = getElement(element.ParentId());
                oldParent.Children.remove(element);
                newParent.Children.push(element);
                element.ParentId(newParent.Id());
                SetDragDrop();
                vm.trackPlanHistory(true);
            }
            $(this).removeClass('label label-success');
        }
    });
    $(".treeview-dropline").droppable({
        over: function (event, ui) {
            var element = getElement($(ui.draggable).attr('id').replace('element_', ''));
            var newParent = getElement($(this).attr('data-parentId'));
            if (canPlace(element, newParent, true))
                $(this).addClass('treeview-dropline-active');
        },
        out: function (event, ui) {
            $(this).removeClass('treeview-dropline-active');
        },
        drop: function (event, ui) {
            var element = getElement($(ui.draggable).attr('id').replace('element_', ''));
            var newParent = getElement($(this).attr('data-parentId'));
            if (canPlace(element, newParent, true)) {
                vm.trackPlanHistory(false);
                ui.draggable.remove();
                var oldParent = getElement(element.ParentId());
                oldParent.Children.remove(element);
                var newIndex = $(this).attr('data-index');
                newParent.Children.splice(newIndex, 0, element);
                element.ParentId(newParent.Id());
                SetDragDrop();
                vm.trackPlanHistory(true);
                //console.log(newParent.Id());
                //console.log(newIndex);
                //console.log(element.Id());
            }
            $(this).removeClass('treeview-dropline-active');
        }
    });
}

function canPlace(element, newParent, dontCheckCurrentParent) {
    if (newParent.Id() == element.Id() ||
        (dontCheckCurrentParent == null && newParent.Id() == element.ParentId()) ||
        !newParent.IsContainer()) return false;
    var pId = newParent.ParentId();
    //Check for element is parent of selected new parent
    while (!(typeof pId === "undefined") && pId != null) {
        var p = getElement(pId);
        if (p == null) {
            pId = null; continue;
        }
        if (p.Id() == element.Id()) return false;
        pId = p.ParentId();
    }
    return true;
}

function initUI() {    
    $("#slider-range").slider({
        range: true,
        min: -100,
        max: 100,
        values: [0, 10],
        slide: function (event, ui) {
            vm.fakeRangeMin(ui.values[0]);
            vm.fakeRangeMax(ui.values[1]);
        }
    });
    $("#slider-interval").slider({
        range: "min",
        value: 5,
        min: 0,
        max: 60,
        slide: function (event, ui) {
            vm.fakeInterval(ui.value);
        }
    });
    $("#slider-fakevalue").slider({
        range: "min",
        value: 10,
        min: -100,
        max: 100,
        slide: function (event, ui) {
            vm.fakeValue(ui.value);
        }
    });
}

function handleKeyboardEvents(e) {
    if (vm.selectedElement() == null || document.activeElement.localName != 'body') return;

    var code = (e.keyCode ? e.keyCode : e.which);
    switch (code) {
        case 46:
            $("#deleteElementModal").modal();
            break;
        case 37:
            e.preventDefault();
            e.stopPropagation();
            MoveElement(-1, 0);
            break;
        case 38:
            e.preventDefault();
            e.stopPropagation();
            MoveElement(0, -1);
            break;
        case 39:
            e.preventDefault();
            e.stopPropagation();
            MoveElement(1, 0);
            break;
        case 40:
            e.preventDefault();
            e.stopPropagation();
            MoveElement(0, 1);
            break;
    }
}