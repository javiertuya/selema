function BranchData() {
    this.position = -1;
    this.nodeLength = -1;
    this.evalFalse = 0;
    this.evalTrue = 0;

    this.init = function(position, nodeLength) {
        this.position = position;
        this.nodeLength = nodeLength;
        return this;
    };

    this.ranCondition = function(result) {
        if (result)
            this.evalTrue++;
        else
            this.evalFalse++;
    };

    this.pathsCovered = function() {
        var paths = 0;
        if (this.evalTrue > 0)
          paths++;
        if (this.evalFalse > 0)
          paths++;
        return paths;
    };

    this.covered = function() {
        return this.evalTrue > 0 && this.evalFalse > 0;
    };

    this.toJSON = function() {
        return '{"position":' + this.position
            + ',"nodeLength":' + this.nodeLength
            + ',"evalFalse":' + this.evalFalse
            + ',"evalTrue":' + this.evalTrue + '}';
    };

    this.message = function(src) {
        if (this.evalTrue === 0 && this.evalFalse === 0)
            return 'Condition never evaluated         :\t' + src + '\n';
        else if (this.evalTrue === 0)
            return 'Condition never evaluated to true :\t' + src + '\n';
        else if (this.evalFalse === 0)
            return 'Condition never evaluated to false:\t' + src + '\n';
        else
            return 'Condition covered';
    };
}

BranchData.fromJson = function(jsonString) {
    var json = eval('(' + jsonString + ')');
    var branchData = new BranchData();
    branchData.init(json.position, json.nodeLength);
    branchData.evalFalse = json.evalFalse;
    branchData.evalTrue = json.evalTrue;
    return branchData;
};

BranchData.fromJsonObject = function(json) {
    var branchData = new BranchData();
    branchData.init(json.position, json.nodeLength);
    branchData.evalFalse = json.evalFalse;
    branchData.evalTrue = json.evalTrue;
    return branchData;
};

function buildBranchMessage(conditions) {
    var message = 'The following was not covered:';
    var i;
    for (i = 0; i < conditions.length; i++) {
        if (conditions[i] !== undefined && conditions[i] !== null && !conditions[i].covered())
            message += '\n- '+ conditions[i].message(conditions[i].src);
    }
    return message;
}

function convertBranchDataConditionArrayToJSON(branchDataConditionArray) {
    var condition, branchDataObject, value;
    var array = [];
    var length = branchDataConditionArray.length;
    for (condition = 0; condition < length; condition++) {
        branchDataObject = branchDataConditionArray[condition];
        if (branchDataObject === undefined || branchDataObject === null) {
            value = 'null';
        } else {
            value = branchDataObject.toJSON();
        }
        array.push(value);
    }
    return '[' + array.join(',') + ']';
}

function convertBranchDataLinesToJSON(branchData) {
    if (branchData === undefined) {
        return '{}'
    }
    var line;
    var json = '';
    for (line in branchData) {
        if (isNaN(line))
            continue;
        if (json !== '')
            json += ',';
        json += '"' + line + '":' + convertBranchDataConditionArrayToJSON(branchData[line]);
    }
    return '{' + json + '}';
}

function convertBranchDataLinesFromJSON(jsonObject) {
    if (jsonObject === undefined) {
        return {};
    }
    var line, branchDataJSON, conditionIndex, condition;
    for (line in jsonObject) {
        branchDataJSON = jsonObject[line];
        if (branchDataJSON !== null) {
            for (conditionIndex = 0; conditionIndex < branchDataJSON.length; conditionIndex ++) {
                condition = branchDataJSON[conditionIndex];
                if (condition !== null) {
                    branchDataJSON[conditionIndex] = BranchData.fromJsonObject(condition);
                }
            }
        }
    }
    return jsonObject;
}
function jscoverage_quote(s) {
    return '"' + s.replace(/[\u0000-\u001f"\\\u007f-\uffff]/g, function (c) {
        switch (c) {
            case '\b':
                return '\\b';
            case '\f':
                return '\\f';
            case '\n':
                return '\\n';
            case '\r':
                return '\\r';
            case '\t':
                return '\\t';
            // IE doesn't support this
            /*
             case '\v':
             return '\\v';
             */
            case '"':
                return '\\"';
            case '\\':
                return '\\\\';
            default:
                return '\\u' + jscoverage_pad(c.charCodeAt(0).toString(16));
        }
    }) + '"';
}

function getArrayJSON(coverage) {
    var array = [];
    if (coverage === undefined)
        return array;

    var length = coverage.length;
    for (var line = 0; line < length; line++) {
        var value = coverage[line];
        if (value === undefined || value === null) {
            value = 'null';
        }
        array.push(value);
    }
    return array;
}

function jscoverage_serializeCoverageToJSON() {
    var json = [];
    for (var file in _$jscoverage) {
        var lineArray = getArrayJSON(_$jscoverage[file].lineData);
        var fnArray = getArrayJSON(_$jscoverage[file].functionData);

        json.push(jscoverage_quote(file) + ':{"lineData":[' + lineArray.join(',') + '],"functionData":[' + fnArray.join(',') + '],"branchData":' + convertBranchDataLinesToJSON(_$jscoverage[file].branchData) + '}');
    }
    return '{' + json.join(',') + '}';
}

function jscoverage_parseCoverageJSON(data) {
    var result = {};
    var json = eval('(' + data + ')');
    var file;
    for (file in json) {
        var fileCoverage = json[file];
        result[file] = {};
        result[file].lineData = fileCoverage.lineData;
        result[file].functionData = fileCoverage.functionData;
        result[file].branchData = convertBranchDataLinesFromJSON(fileCoverage.branchData);
    }
    return result;
}

function jscoverage_pad(s) {
    return '0000'.substr(s.length) + s;
}

function jscoverage_html_escape(s) {
    return s.replace(/[<>\&\"\']/g, function (c) {
        return '&#' + c.charCodeAt(0) + ';';
    });
}
if (typeof(_$jscoverage) === "undefined" && (typeof(Storage) !== "undefined") && typeof(localStorage["jscover"]) !== "undefined")
    _$jscoverage = jscoverage_parseCoverageJSON(localStorage["jscover"]);
if (typeof(jscoverbeforeunload) === "undefined") {
    jscoverbeforeunload = (window.onbeforeunload) ? window.onbeforeunload : function () {};
    window.onbeforeunload = function () {
        jscoverbeforeunload();
        if ((typeof(_$jscoverage) !== "undefined") && (typeof(Storage) !== "undefined"))
            localStorage["jscover"] = jscoverage_serializeCoverageToJSON();
    };
}
var jsCover_isolateBrowser = false;
if (!jsCover_isolateBrowser) {
    try {
        if (typeof top === 'object' && top !== null && typeof top.opener === 'object' && top.opener !== null) {
            // this is a browser window that was opened from another window

            if (!top.opener._$jscoverage) {
                top.opener._$jscoverage = {};
            }
        }
    } catch (e) {
    }

    try {
        if (typeof top === 'object' && top !== null) {
            // this is a browser window

            try {
                if (typeof top.opener === 'object' && top.opener !== null && top.opener._$jscoverage) {
                    top._$jscoverage = top.opener._$jscoverage;
                }
            } catch (e) {
            }

            if (!top._$jscoverage) {
                top._$jscoverage = {};
            }
        }
    } catch (e) {
    }

    try {
        if (typeof top === 'object' && top !== null && top._$jscoverage) {
            this._$jscoverage = top._$jscoverage;
        }
    } catch (e) {
    }
}
if (!this._$jscoverage) {
    this._$jscoverage = {};
}
if (! _$jscoverage['/actions.js']) {
  _$jscoverage['/actions.js'] = {};
  _$jscoverage['/actions.js'].lineData = [];
  _$jscoverage['/actions.js'].lineData[2] = 0;
  _$jscoverage['/actions.js'].lineData[3] = 0;
  _$jscoverage['/actions.js'].lineData[4] = 0;
  _$jscoverage['/actions.js'].lineData[6] = 0;
  _$jscoverage['/actions.js'].lineData[7] = 0;
  _$jscoverage['/actions.js'].lineData[8] = 0;
  _$jscoverage['/actions.js'].lineData[10] = 0;
  _$jscoverage['/actions.js'].lineData[11] = 0;
  _$jscoverage['/actions.js'].lineData[12] = 0;
  _$jscoverage['/actions.js'].lineData[13] = 0;
  _$jscoverage['/actions.js'].lineData[14] = 0;
}
if (! _$jscoverage['/actions.js'].functionData) {
  _$jscoverage['/actions.js'].functionData = [];
  _$jscoverage['/actions.js'].functionData[0] = 0;
  _$jscoverage['/actions.js'].functionData[1] = 0;
  _$jscoverage['/actions.js'].functionData[2] = 0;
}
if (! _$jscoverage['/actions.js'].branchData) {
  _$jscoverage['/actions.js'].branchData = {};
  _$jscoverage['/actions.js'].branchData['12'] = [];
  _$jscoverage['/actions.js'].branchData['12'][1] = new BranchData();
}
_$jscoverage['/actions.js'].branchData['12'][1].init(8, 9);
function visit1_12_1(result) {
  _$jscoverage['/actions.js'].branchData['12'][1].ranCondition(result);
  return result;
}
_$jscoverage['/actions.js'].lineData[2]++;
function doAlert() {
  _$jscoverage['/actions.js'].functionData[0]++;
  _$jscoverage['/actions.js'].lineData[3]++;
  alert('alert msg');
  _$jscoverage['/actions.js'].lineData[4]++;
  document.getElementById('spanAlert').textContent = 'clicked';
}
_$jscoverage['/actions.js'].lineData[6]++;
function doConfirm() {
  _$jscoverage['/actions.js'].functionData[1]++;
  _$jscoverage['/actions.js'].lineData[7]++;
  var ret = confirm('confirm msg');
  _$jscoverage['/actions.js'].lineData[8]++;
  document.getElementById('spanConfirm').textContent = ret.toString();
}
_$jscoverage['/actions.js'].lineData[10]++;
function doPrompt() {
  _$jscoverage['/actions.js'].functionData[2]++;
  _$jscoverage['/actions.js'].lineData[11]++;
  var ret = prompt('confirm msg');
  _$jscoverage['/actions.js'].lineData[12]++;
  if (visit1_12_1(ret == null)) {
    _$jscoverage['/actions.js'].lineData[13]++;
    ret = 'NULL';
  }
  _$jscoverage['/actions.js'].lineData[14]++;
  document.getElementById('spanPrompt').textContent = ret;
}
