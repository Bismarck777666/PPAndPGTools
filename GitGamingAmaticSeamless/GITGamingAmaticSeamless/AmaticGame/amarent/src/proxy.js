//sessionStorage
let sStorage = function() {
    this.sessionValue1 = getUrlParam('lang');
    this.sessionValue2 = getUrlParam('hash');
    this.sessionValue5 = getUrlParam('setVar5');
    this.sessionValue6 = "true";
    this.sessionValue14 = "false";
    this.sessionValue15 = "false";
    this.sessionValue54 = getUrlParam('sessionValue54');
    this.sessionValue55 = getUrlParam('sessionValue55');
    this.setVar6 = getUrlParam('setVar6');
    this.setVar11 = getUrlParam('setVar11');
    this.setVar21 = getUrlParam('setVar21');
    this.setVar24 = getUrlParam('setVar24');
    this.setVar1007 = getUrlParam('setVar1007');
    this.setVar22 = getUrlParam('setVar22');
    this.setVar23 = getUrlParam('setVar23');
    this.removeItem = function(item) {
        if (this[item] !== undefined)
            delete this[item];
    };
    this.clear = function() {

    };
    this.getItem = function(item) {
        if (this[item] !== undefined)
            return this[item];
        else
            return undefined;
    }
    this.setItem = function(item, v) {
        this[item] = v;
    }
}
//localStorage
let lStorage = function() {
    this.removeItem = function(item) {
        if (this[item] !== undefined)
            delete this[item];
    };
    this.clear = function() {

    };
    this.getItem = function(item) {
        if (this[item] !== undefined)
            return this[item];
        else
            return undefined;
    }
    this.setItem = function(item, v) {
        this[item] = v;
    }
};

let myLStorage = new lStorage();
let mySStorage = new sStorage();

var proxyStorage = new Proxy(mySStorage, {
    get: function(target, name, receiver) {
        //console.log("Name of requested property: " + name);
        var rv = target[name];
        if (rv === undefined) {
            //console.log("There is no such thing as " + name + ".")
            rv = "";
        }
        //console.log(rv);
        return rv;
    }
});

var proxyLStorage = new Proxy(myLStorage, {
    get: function(target, name, receiver) {
        //console.log("Name of requested property: " + name);
        var rv = target[name];
        if (rv === undefined) {
            //console.log("There is no such thing as " + name + ".")
            rv = "";
        }
        //console.log(rv);
        return rv;
    }
});