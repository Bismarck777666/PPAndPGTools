var Ud = Object.defineProperty
  , jd = Object.defineProperties;
var Bd = Object.getOwnPropertyDescriptors;
var Ha = Object.getOwnPropertySymbols;
var zd = Object.prototype.hasOwnProperty
  , Vd = Object.prototype.propertyIsEnumerable;
var di = (t, e) => (e = Symbol[t]) ? e : Symbol.for("Symbol." + t)
  , Hd = t => {
    throw TypeError(t)
}
;
var Ka = (t, e, n) => e in t ? Ud(t, e, {
    enumerable: !0,
    configurable: !0,
    writable: !0,
    value: n
}) : t[e] = n
  , Bn = (t, e) => {
    for (var n in e || (e = {}))
        zd.call(e, n) && Ka(t, n, e[n]);
    if (Ha)
        for (var n of Ha(e))
            Vd.call(e, n) && Ka(t, n, e[n]);
    return t
}
  , Is = (t, e) => jd(t, Bd(e));
var Kd = (t, e) => () => (e || t((e = {
    exports: {}
}).exports, e),
e.exports);
var At = (t, e, n) => new Promise( (i, s) => {
    var r = l => {
        try {
            a(n.next(l))
        } catch (c) {
            s(c)
        }
    }
      , o = l => {
        try {
            a(n.throw(l))
        } catch (c) {
            s(c)
        }
    }
      , a = l => l.done ? i(l.value) : Promise.resolve(l.value).then(r, o);
    a((n = n.apply(t, e)).next())
}
)
  , zn = function(t, e) {
    this[0] = t,
    this[1] = e
}
  , Nr = (t, e, n) => {
    var i = (o, a, l, c) => {
        try {
            var u = n[o](a)
              , f = (a = u.value)instanceof zn
              , d = u.done;
            Promise.resolve(f ? a[0] : a).then(_ => f ? i(o === "return" ? o : "next", a[1] ? {
                done: _.done,
                value: _.value
            } : _, l, c) : l({
                value: _,
                done: d
            })).catch(_ => i("throw", _, l, c))
        } catch (_) {
            c(_)
        }
    }
      , s = o => r[o] = a => new Promise( (l, c) => i(o, a, l, c))
      , r = {};
    return n = n.apply(t, e),
    r[di("asyncIterator")] = () => r,
    s("next"),
    s("throw"),
    s("return"),
    r
}
  , Dr = t => {
    var e = t[di("asyncIterator")], n = !1, i, s = {};
    return e == null ? (e = t[di("iterator")](),
    i = r => s[r] = o => e[r](o)) : (e = e.call(t),
    i = r => s[r] = o => {
        if (n) {
            if (n = !1,
            r === "throw")
                throw o;
            return o
        }
        return n = !0,
        {
            done: !1,
            value: new zn(new Promise(a => {
                var l = e[r](o);
                l instanceof Object || Hd("Object expected"),
                a(l)
            }
            ),1)
        }
    }
    ),
    s[di("iterator")] = () => s,
    i("next"),
    "throw"in e ? i("throw") : s.throw = r => {
        throw r
    }
    ,
    "return"in e && i("return"),
    s
}
  , qa = (t, e, n) => (e = t[di("asyncIterator")]) ? e.call(t) : (t = t[di("iterator")](),
e = {},
n = (i, s) => (s = t[i]) && (e[i] = r => new Promise( (o, a, l) => (r = s.call(t, r),
l = r.done,
Promise.resolve(r.value).then(c => o({
    value: c,
    done: l
}), a)))),
n("next"),
n("return"),
e);
var X2 = Kd(xs => {
    (function() {
        const e = document.createElement("link").relList;
        if (e && e.supports && e.supports("modulepreload"))
            return;
        for (const s of document.querySelectorAll('link[rel="modulepreload"]'))
            i(s);
        new MutationObserver(s => {
            for (const r of s)
                if (r.type === "childList")
                    for (const o of r.addedNodes)
                        o.tagName === "LINK" && o.rel === "modulepreload" && i(o)
        }
        ).observe(document, {
            childList: !0,
            subtree: !0
        });
        function n(s) {
            const r = {};
            return s.integrity && (r.integrity = s.integrity),
            s.referrerPolicy && (r.referrerPolicy = s.referrerPolicy),
            s.crossOrigin === "use-credentials" ? r.credentials = "include" : s.crossOrigin === "anonymous" ? r.credentials = "omit" : r.credentials = "same-origin",
            r
        }
        function i(s) {
            if (s.ep)
                return;
            s.ep = !0;
            const r = n(s);
            fetch(s.href, r)
        }
    }
    )();
    /**
* @vue/shared v3.5.13
* (c) 2018-present Yuxi (Evan) You and Vue contributors
* @license MIT
**/
    /*! #__NO_SIDE_EFFECTS__ */
    function Xo(t) {
        const e = Object.create(null);
        for (const n of t.split(","))
            e[n] = 1;
        return n => n in e
    }
    const Ae = {}
      , ki = []
      , en = () => {}
      , qd = () => !1
      , pr = t => t.charCodeAt(0) === 111 && t.charCodeAt(1) === 110 && (t.charCodeAt(2) > 122 || t.charCodeAt(2) < 97)
      , Jo = t => t.startsWith("onUpdate:")
      , it = Object.assign
      , Qo = (t, e) => {
        const n = t.indexOf(e);
        n > -1 && t.splice(n, 1)
    }
      , Wd = Object.prototype.hasOwnProperty
      , pe = (t, e) => Wd.call(t, e)
      , ie = Array.isArray
      , Ai = t => _r(t) === "[object Map]"
      , Lc = t => _r(t) === "[object Set]"
      , re = t => typeof t == "function"
      , Be = t => typeof t == "string"
      , Ln = t => typeof t == "symbol"
      , xe = t => t !== null && typeof t == "object"
      , Nc = t => (xe(t) || re(t)) && re(t.then) && re(t.catch)
      , Dc = Object.prototype.toString
      , _r = t => Dc.call(t)
      , Yd = t => _r(t).slice(8, -1)
      , Mc = t => _r(t) === "[object Object]"
      , Zo = t => Be(t) && t !== "NaN" && t[0] !== "-" && "" + parseInt(t, 10) === t
      , Yi = Xo(",key,ref,ref_for,ref_key,onVnodeBeforeMount,onVnodeMounted,onVnodeBeforeUpdate,onVnodeUpdated,onVnodeBeforeUnmount,onVnodeUnmounted")
      , hr = t => {
        const e = Object.create(null);
        return n => e[n] || (e[n] = t(n))
    }
      , Xd = /-(\w)/g
      , xn = hr(t => t.replace(Xd, (e, n) => n ? n.toUpperCase() : ""))
      , Jd = /\B([A-Z])/g
      , ui = hr(t => t.replace(Jd, "-$1").toLowerCase())
      , Fc = hr(t => t.charAt(0).toUpperCase() + t.slice(1))
      , Mr = hr(t => t ? "on".concat(Fc(t)) : "")
      , wn = (t, e) => !Object.is(t, e)
      , Fr = (t, ...e) => {
        for (let n = 0; n < t.length; n++)
            t[n](...e)
    }
      , Gc = (t, e, n, i=!1) => {
        Object.defineProperty(t, e, {
            configurable: !0,
            enumerable: !1,
            writable: i,
            value: n
        })
    }
      , Qd = t => {
        const e = parseFloat(t);
        return isNaN(e) ? t : e
    }
    ;
    let Wa;
    const mr = () => Wa || (Wa = typeof globalThis != "undefined" ? globalThis : typeof self != "undefined" ? self : typeof window != "undefined" ? window : typeof global != "undefined" ? global : {});
    function os(t) {
        if (ie(t)) {
            const e = {};
            for (let n = 0; n < t.length; n++) {
                const i = t[n]
                  , s = Be(i) ? np(i) : os(i);
                if (s)
                    for (const r in s)
                        e[r] = s[r]
            }
            return e
        } else if (Be(t) || xe(t))
            return t
    }
    const Zd = /;(?![^(]*\))/g
      , ep = /:([^]+)/
      , tp = /\/\*[^]*?\*\//g;
    function np(t) {
        const e = {};
        return t.replace(tp, "").split(Zd).forEach(n => {
            if (n) {
                const i = n.split(ep);
                i.length > 1 && (e[i[0].trim()] = i[1].trim())
            }
        }
        ),
        e
    }
    function ce(t) {
        let e = "";
        if (Be(t))
            e = t;
        else if (ie(t))
            for (let n = 0; n < t.length; n++) {
                const i = ce(t[n]);
                i && (e += i + " ")
            }
        else if (xe(t))
            for (const n in t)
                t[n] && (e += n + " ");
        return e.trim()
    }
    const ip = "itemscope,allowfullscreen,formnovalidate,ismap,nomodule,novalidate,readonly"
      , sp = Xo(ip);
    function Uc(t) {
        return !!t || t === ""
    }
    const jc = t => !!(t && t.__v_isRef === !0)
      , Xe = t => Be(t) ? t : t == null ? "" : ie(t) || xe(t) && (t.toString === Dc || !re(t.toString)) ? jc(t) ? Xe(t.value) : JSON.stringify(t, Bc, 2) : String(t)
      , Bc = (t, e) => jc(e) ? Bc(t, e.value) : Ai(e) ? {
        ["Map(".concat(e.size, ")")]: [...e.entries()].reduce( (n, [i,s], r) => (n[Gr(i, r) + " =>"] = s,
        n), {})
    } : Lc(e) ? {
        ["Set(".concat(e.size, ")")]: [...e.values()].map(n => Gr(n))
    } : Ln(e) ? Gr(e) : xe(e) && !ie(e) && !Mc(e) ? String(e) : e
      , Gr = (t, e="") => {
        var n;
        return Ln(t) ? "Symbol(".concat((n = t.description) != null ? n : e, ")") : t
    }
    ;
    /**
* @vue/reactivity v3.5.13
* (c) 2018-present Yuxi (Evan) You and Vue contributors
* @license MIT
**/
    let Tt;
    class zc {
        constructor(e=!1) {
            this.detached = e,
            this._active = !0,
            this.effects = [],
            this.cleanups = [],
            this._isPaused = !1,
            this.parent = Tt,
            !e && Tt && (this.index = (Tt.scopes || (Tt.scopes = [])).push(this) - 1)
        }
        get active() {
            return this._active
        }
        pause() {
            if (this._active) {
                this._isPaused = !0;
                let e, n;
                if (this.scopes)
                    for (e = 0,
                    n = this.scopes.length; e < n; e++)
                        this.scopes[e].pause();
                for (e = 0,
                n = this.effects.length; e < n; e++)
                    this.effects[e].pause()
            }
        }
        resume() {
            if (this._active && this._isPaused) {
                this._isPaused = !1;
                let e, n;
                if (this.scopes)
                    for (e = 0,
                    n = this.scopes.length; e < n; e++)
                        this.scopes[e].resume();
                for (e = 0,
                n = this.effects.length; e < n; e++)
                    this.effects[e].resume()
            }
        }
        run(e) {
            if (this._active) {
                const n = Tt;
                try {
                    return Tt = this,
                    e()
                } finally {
                    Tt = n
                }
            }
        }
        on() {
            Tt = this
        }
        off() {
            Tt = this.parent
        }
        stop(e) {
            if (this._active) {
                this._active = !1;
                let n, i;
                for (n = 0,
                i = this.effects.length; n < i; n++)
                    this.effects[n].stop();
                for (this.effects.length = 0,
                n = 0,
                i = this.cleanups.length; n < i; n++)
                    this.cleanups[n]();
                if (this.cleanups.length = 0,
                this.scopes) {
                    for (n = 0,
                    i = this.scopes.length; n < i; n++)
                        this.scopes[n].stop(!0);
                    this.scopes.length = 0
                }
                if (!this.detached && this.parent && !e) {
                    const s = this.parent.scopes.pop();
                    s && s !== this && (this.parent.scopes[this.index] = s,
                    s.index = this.index)
                }
                this.parent = void 0
            }
        }
    }
    function rp(t) {
        return new zc(t)
    }
    function op() {
        return Tt
    }
    let ke;
    const Ur = new WeakSet;
    class Vc {
        constructor(e) {
            this.fn = e,
            this.deps = void 0,
            this.depsTail = void 0,
            this.flags = 5,
            this.next = void 0,
            this.cleanup = void 0,
            this.scheduler = void 0,
            Tt && Tt.active && Tt.effects.push(this)
        }
        pause() {
            this.flags |= 64
        }
        resume() {
            this.flags & 64 && (this.flags &= -65,
            Ur.has(this) && (Ur.delete(this),
            this.trigger()))
        }
        notify() {
            this.flags & 2 && !(this.flags & 32) || this.flags & 8 || Kc(this)
        }
        run() {
            if (!(this.flags & 1))
                return this.fn();
            this.flags |= 2,
            Ya(this),
            qc(this);
            const e = ke
              , n = Bt;
            ke = this,
            Bt = !0;
            try {
                return this.fn()
            } finally {
                Wc(this),
                ke = e,
                Bt = n,
                this.flags &= -3
            }
        }
        stop() {
            if (this.flags & 1) {
                for (let e = this.deps; e; e = e.nextDep)
                    na(e);
                this.deps = this.depsTail = void 0,
                Ya(this),
                this.onStop && this.onStop(),
                this.flags &= -2
            }
        }
        trigger() {
            this.flags & 64 ? Ur.add(this) : this.scheduler ? this.scheduler() : this.runIfDirty()
        }
        runIfDirty() {
            lo(this) && this.run()
        }
        get dirty() {
            return lo(this)
        }
    }
    let Hc = 0, Xi, Ji;
    function Kc(t, e=!1) {
        if (t.flags |= 8,
        e) {
            t.next = Ji,
            Ji = t;
            return
        }
        t.next = Xi,
        Xi = t
    }
    function ea() {
        Hc++
    }
    function ta() {
        if (--Hc > 0)
            return;
        if (Ji) {
            let e = Ji;
            for (Ji = void 0; e; ) {
                const n = e.next;
                e.next = void 0,
                e.flags &= -9,
                e = n
            }
        }
        let t;
        for (; Xi; ) {
            let e = Xi;
            for (Xi = void 0; e; ) {
                const n = e.next;
                if (e.next = void 0,
                e.flags &= -9,
                e.flags & 1)
                    try {
                        e.trigger()
                    } catch (i) {
                        t || (t = i)
                    }
                e = n
            }
        }
        if (t)
            throw t
    }
    function qc(t) {
        for (let e = t.deps; e; e = e.nextDep)
            e.version = -1,
            e.prevActiveLink = e.dep.activeLink,
            e.dep.activeLink = e
    }
    function Wc(t) {
        let e, n = t.depsTail, i = n;
        for (; i; ) {
            const s = i.prevDep;
            i.version === -1 ? (i === n && (n = s),
            na(i),
            ap(i)) : e = i,
            i.dep.activeLink = i.prevActiveLink,
            i.prevActiveLink = void 0,
            i = s
        }
        t.deps = e,
        t.depsTail = n
    }
    function lo(t) {
        for (let e = t.deps; e; e = e.nextDep)
            if (e.dep.version !== e.version || e.dep.computed && (Yc(e.dep.computed) || e.dep.version !== e.version))
                return !0;
        return !!t._dirty
    }
    function Yc(t) {
        if (t.flags & 4 && !(t.flags & 16) || (t.flags &= -17,
        t.globalVersion === as))
            return;
        t.globalVersion = as;
        const e = t.dep;
        if (t.flags |= 2,
        e.version > 0 && !t.isSSR && t.deps && !lo(t)) {
            t.flags &= -3;
            return
        }
        const n = ke
          , i = Bt;
        ke = t,
        Bt = !0;
        try {
            qc(t);
            const s = t.fn(t._value);
            (e.version === 0 || wn(s, t._value)) && (t._value = s,
            e.version++)
        } catch (s) {
            throw e.version++,
            s
        } finally {
            ke = n,
            Bt = i,
            Wc(t),
            t.flags &= -3
        }
    }
    function na(t, e=!1) {
        const {dep: n, prevSub: i, nextSub: s} = t;
        if (i && (i.nextSub = s,
        t.prevSub = void 0),
        s && (s.prevSub = i,
        t.nextSub = void 0),
        n.subs === t && (n.subs = i,
        !i && n.computed)) {
            n.computed.flags &= -5;
            for (let r = n.computed.deps; r; r = r.nextDep)
                na(r, !0)
        }
        !e && !--n.sc && n.map && n.map.delete(n.key)
    }
    function ap(t) {
        const {prevDep: e, nextDep: n} = t;
        e && (e.nextDep = n,
        t.prevDep = void 0),
        n && (n.prevDep = e,
        t.nextDep = void 0)
    }
    let Bt = !0;
    const Xc = [];
    function Nn() {
        Xc.push(Bt),
        Bt = !1
    }
    function Dn() {
        const t = Xc.pop();
        Bt = t === void 0 ? !0 : t
    }
    function Ya(t) {
        const {cleanup: e} = t;
        if (t.cleanup = void 0,
        e) {
            const n = ke;
            ke = void 0;
            try {
                e()
            } finally {
                ke = n
            }
        }
    }
    let as = 0;
    class lp {
        constructor(e, n) {
            this.sub = e,
            this.dep = n,
            this.version = n.version,
            this.nextDep = this.prevDep = this.nextSub = this.prevSub = this.prevActiveLink = void 0
        }
    }
    class ia {
        constructor(e) {
            this.computed = e,
            this.version = 0,
            this.activeLink = void 0,
            this.subs = void 0,
            this.map = void 0,
            this.key = void 0,
            this.sc = 0
        }
        track(e) {
            if (!ke || !Bt || ke === this.computed)
                return;
            let n = this.activeLink;
            if (n === void 0 || n.sub !== ke)
                n = this.activeLink = new lp(ke,this),
                ke.deps ? (n.prevDep = ke.depsTail,
                ke.depsTail.nextDep = n,
                ke.depsTail = n) : ke.deps = ke.depsTail = n,
                Jc(n);
            else if (n.version === -1 && (n.version = this.version,
            n.nextDep)) {
                const i = n.nextDep;
                i.prevDep = n.prevDep,
                n.prevDep && (n.prevDep.nextDep = i),
                n.prevDep = ke.depsTail,
                n.nextDep = void 0,
                ke.depsTail.nextDep = n,
                ke.depsTail = n,
                ke.deps === n && (ke.deps = i)
            }
            return n
        }
        trigger(e) {
            this.version++,
            as++,
            this.notify(e)
        }
        notify(e) {
            ea();
            try {
                for (let n = this.subs; n; n = n.prevSub)
                    n.sub.notify() && n.sub.dep.notify()
            } finally {
                ta()
            }
        }
    }
    function Jc(t) {
        if (t.dep.sc++,
        t.sub.flags & 4) {
            const e = t.dep.computed;
            if (e && !t.dep.subs) {
                e.flags |= 20;
                for (let i = e.deps; i; i = i.nextDep)
                    Jc(i)
            }
            const n = t.dep.subs;
            n !== t && (t.prevSub = n,
            n && (n.nextSub = t)),
            t.dep.subs = t
        }
    }
    const co = new WeakMap
      , ei = Symbol("")
      , uo = Symbol("")
      , ls = Symbol("");
    function Je(t, e, n) {
        if (Bt && ke) {
            let i = co.get(t);
            i || co.set(t, i = new Map);
            let s = i.get(n);
            s || (i.set(n, s = new ia),
            s.map = i,
            s.key = n),
            s.track()
        }
    }
    function _n(t, e, n, i, s, r) {
        const o = co.get(t);
        if (!o) {
            as++;
            return
        }
        const a = l => {
            l && l.trigger()
        }
        ;
        if (ea(),
        e === "clear")
            o.forEach(a);
        else {
            const l = ie(t)
              , c = l && Zo(n);
            if (l && n === "length") {
                const u = Number(i);
                o.forEach( (f, d) => {
                    (d === "length" || d === ls || !Ln(d) && d >= u) && a(f)
                }
                )
            } else
                switch ((n !== void 0 || o.has(void 0)) && a(o.get(n)),
                c && a(o.get(ls)),
                e) {
                case "add":
                    l ? c && a(o.get("length")) : (a(o.get(ei)),
                    Ai(t) && a(o.get(uo)));
                    break;
                case "delete":
                    l || (a(o.get(ei)),
                    Ai(t) && a(o.get(uo)));
                    break;
                case "set":
                    Ai(t) && a(o.get(ei));
                    break
                }
        }
        ta()
    }
    function pi(t) {
        const e = de(t);
        return e === t ? e : (Je(e, "iterate", ls),
        zt(t) ? e : e.map(lt))
    }
    function sa(t) {
        return Je(t = de(t), "iterate", ls),
        t
    }
    const cp = {
        __proto__: null,
        [Symbol.iterator]() {
            return jr(this, Symbol.iterator, lt)
        },
        concat(...t) {
            return pi(this).concat(...t.map(e => ie(e) ? pi(e) : e))
        },
        entries() {
            return jr(this, "entries", t => (t[1] = lt(t[1]),
            t))
        },
        every(t, e) {
            return an(this, "every", t, e, void 0, arguments)
        },
        filter(t, e) {
            return an(this, "filter", t, e, n => n.map(lt), arguments)
        },
        find(t, e) {
            return an(this, "find", t, e, lt, arguments)
        },
        findIndex(t, e) {
            return an(this, "findIndex", t, e, void 0, arguments)
        },
        findLast(t, e) {
            return an(this, "findLast", t, e, lt, arguments)
        },
        findLastIndex(t, e) {
            return an(this, "findLastIndex", t, e, void 0, arguments)
        },
        forEach(t, e) {
            return an(this, "forEach", t, e, void 0, arguments)
        },
        includes(...t) {
            return Br(this, "includes", t)
        },
        indexOf(...t) {
            return Br(this, "indexOf", t)
        },
        join(t) {
            return pi(this).join(t)
        },
        lastIndexOf(...t) {
            return Br(this, "lastIndexOf", t)
        },
        map(t, e) {
            return an(this, "map", t, e, void 0, arguments)
        },
        pop() {
            return ji(this, "pop")
        },
        push(...t) {
            return ji(this, "push", t)
        },
        reduce(t, ...e) {
            return Xa(this, "reduce", t, e)
        },
        reduceRight(t, ...e) {
            return Xa(this, "reduceRight", t, e)
        },
        shift() {
            return ji(this, "shift")
        },
        some(t, e) {
            return an(this, "some", t, e, void 0, arguments)
        },
        splice(...t) {
            return ji(this, "splice", t)
        },
        toReversed() {
            return pi(this).toReversed()
        },
        toSorted(t) {
            return pi(this).toSorted(t)
        },
        toSpliced(...t) {
            return pi(this).toSpliced(...t)
        },
        unshift(...t) {
            return ji(this, "unshift", t)
        },
        values() {
            return jr(this, "values", lt)
        }
    };
    function jr(t, e, n) {
        const i = sa(t)
          , s = i[e]();
        return i !== t && !zt(t) && (s._next = s.next,
        s.next = () => {
            const r = s._next();
            return r.value && (r.value = n(r.value)),
            r
        }
        ),
        s
    }
    const up = Array.prototype;
    function an(t, e, n, i, s, r) {
        const o = sa(t)
          , a = o !== t && !zt(t)
          , l = o[e];
        if (l !== up[e]) {
            const f = l.apply(t, r);
            return a ? lt(f) : f
        }
        let c = n;
        o !== t && (a ? c = function(f, d) {
            return n.call(this, lt(f), d, t)
        }
        : n.length > 2 && (c = function(f, d) {
            return n.call(this, f, d, t)
        }
        ));
        const u = l.call(o, c, i);
        return a && s ? s(u) : u
    }
    function Xa(t, e, n, i) {
        const s = sa(t);
        let r = n;
        return s !== t && (zt(t) ? n.length > 3 && (r = function(o, a, l) {
            return n.call(this, o, a, l, t)
        }
        ) : r = function(o, a, l) {
            return n.call(this, o, lt(a), l, t)
        }
        ),
        s[e](r, ...i)
    }
    function Br(t, e, n) {
        const i = de(t);
        Je(i, "iterate", ls);
        const s = i[e](...n);
        return (s === -1 || s === !1) && la(n[0]) ? (n[0] = de(n[0]),
        i[e](...n)) : s
    }
    function ji(t, e, n=[]) {
        Nn(),
        ea();
        const i = de(t)[e].apply(t, n);
        return ta(),
        Dn(),
        i
    }
    const fp = Xo("__proto__,__v_isRef,__isVue")
      , Qc = new Set(Object.getOwnPropertyNames(Symbol).filter(t => t !== "arguments" && t !== "caller").map(t => Symbol[t]).filter(Ln));
    function dp(t) {
        Ln(t) || (t = String(t));
        const e = de(this);
        return Je(e, "has", t),
        e.hasOwnProperty(t)
    }
    class Zc {
        constructor(e=!1, n=!1) {
            this._isReadonly = e,
            this._isShallow = n
        }
        get(e, n, i) {
            if (n === "__v_skip")
                return e.__v_skip;
            const s = this._isReadonly
              , r = this._isShallow;
            if (n === "__v_isReactive")
                return !s;
            if (n === "__v_isReadonly")
                return s;
            if (n === "__v_isShallow")
                return r;
            if (n === "__v_raw")
                return i === (s ? r ? Ap : iu : r ? nu : tu).get(e) || Object.getPrototypeOf(e) === Object.getPrototypeOf(i) ? e : void 0;
            const o = ie(e);
            if (!s) {
                let l;
                if (o && (l = cp[n]))
                    return l;
                if (n === "hasOwnProperty")
                    return dp
            }
            const a = Reflect.get(e, n, qe(e) ? e : i);
            return (Ln(n) ? Qc.has(n) : fp(n)) || (s || Je(e, "get", n),
            r) ? a : qe(a) ? o && Zo(n) ? a : a.value : xe(a) ? s ? su(a) : oa(a) : a
        }
    }
    class eu extends Zc {
        constructor(e=!1) {
            super(!1, e)
        }
        set(e, n, i, s) {
            let r = e[n];
            if (!this._isShallow) {
                const l = oi(r);
                if (!zt(i) && !oi(i) && (r = de(r),
                i = de(i)),
                !ie(e) && qe(r) && !qe(i))
                    return l ? !1 : (r.value = i,
                    !0)
            }
            const o = ie(e) && Zo(n) ? Number(n) < e.length : pe(e, n)
              , a = Reflect.set(e, n, i, qe(e) ? e : s);
            return e === de(s) && (o ? wn(i, r) && _n(e, "set", n, i) : _n(e, "add", n, i)),
            a
        }
        deleteProperty(e, n) {
            const i = pe(e, n);
            e[n];
            const s = Reflect.deleteProperty(e, n);
            return s && i && _n(e, "delete", n, void 0),
            s
        }
        has(e, n) {
            const i = Reflect.has(e, n);
            return (!Ln(n) || !Qc.has(n)) && Je(e, "has", n),
            i
        }
        ownKeys(e) {
            return Je(e, "iterate", ie(e) ? "length" : ei),
            Reflect.ownKeys(e)
        }
    }
    class pp extends Zc {
        constructor(e=!1) {
            super(!0, e)
        }
        set(e, n) {
            return !0
        }
        deleteProperty(e, n) {
            return !0
        }
    }
    const _p = new eu
      , hp = new pp
      , mp = new eu(!0)
      , fo = t => t
      , Cs = t => Reflect.getPrototypeOf(t);
    function gp(t, e, n) {
        return function(...i) {
            const s = this.__v_raw
              , r = de(s)
              , o = Ai(r)
              , a = t === "entries" || t === Symbol.iterator && o
              , l = t === "keys" && o
              , c = s[t](...i)
              , u = n ? fo : e ? po : lt;
            return !e && Je(r, "iterate", l ? uo : ei),
            {
                next() {
                    const {value: f, done: d} = c.next();
                    return d ? {
                        value: f,
                        done: d
                    } : {
                        value: a ? [u(f[0]), u(f[1])] : u(f),
                        done: d
                    }
                },
                [Symbol.iterator]() {
                    return this
                }
            }
        }
    }
    function Rs(t) {
        return function(...e) {
            return t === "delete" ? !1 : t === "clear" ? void 0 : this
        }
    }
    function bp(t, e) {
        const n = {
            get(s) {
                const r = this.__v_raw
                  , o = de(r)
                  , a = de(s);
                t || (wn(s, a) && Je(o, "get", s),
                Je(o, "get", a));
                const {has: l} = Cs(o)
                  , c = e ? fo : t ? po : lt;
                if (l.call(o, s))
                    return c(r.get(s));
                if (l.call(o, a))
                    return c(r.get(a));
                r !== o && r.get(s)
            },
            get size() {
                const s = this.__v_raw;
                return !t && Je(de(s), "iterate", ei),
                Reflect.get(s, "size", s)
            },
            has(s) {
                const r = this.__v_raw
                  , o = de(r)
                  , a = de(s);
                return t || (wn(s, a) && Je(o, "has", s),
                Je(o, "has", a)),
                s === a ? r.has(s) : r.has(s) || r.has(a)
            },
            forEach(s, r) {
                const o = this
                  , a = o.__v_raw
                  , l = de(a)
                  , c = e ? fo : t ? po : lt;
                return !t && Je(l, "iterate", ei),
                a.forEach( (u, f) => s.call(r, c(u), c(f), o))
            }
        };
        return it(n, t ? {
            add: Rs("add"),
            set: Rs("set"),
            delete: Rs("delete"),
            clear: Rs("clear")
        } : {
            add(s) {
                !e && !zt(s) && !oi(s) && (s = de(s));
                const r = de(this);
                return Cs(r).has.call(r, s) || (r.add(s),
                _n(r, "add", s, s)),
                this
            },
            set(s, r) {
                !e && !zt(r) && !oi(r) && (r = de(r));
                const o = de(this)
                  , {has: a, get: l} = Cs(o);
                let c = a.call(o, s);
                c || (s = de(s),
                c = a.call(o, s));
                const u = l.call(o, s);
                return o.set(s, r),
                c ? wn(r, u) && _n(o, "set", s, r) : _n(o, "add", s, r),
                this
            },
            delete(s) {
                const r = de(this)
                  , {has: o, get: a} = Cs(r);
                let l = o.call(r, s);
                l || (s = de(s),
                l = o.call(r, s)),
                a && a.call(r, s);
                const c = r.delete(s);
                return l && _n(r, "delete", s, void 0),
                c
            },
            clear() {
                const s = de(this)
                  , r = s.size !== 0
                  , o = s.clear();
                return r && _n(s, "clear", void 0, void 0),
                o
            }
        }),
        ["keys", "values", "entries", Symbol.iterator].forEach(s => {
            n[s] = gp(s, t, e)
        }
        ),
        n
    }
    function ra(t, e) {
        const n = bp(t, e);
        return (i, s, r) => s === "__v_isReactive" ? !t : s === "__v_isReadonly" ? t : s === "__v_raw" ? i : Reflect.get(pe(n, s) && s in i ? n : i, s, r)
    }
    const yp = {
        get: ra(!1, !1)
    }
      , vp = {
        get: ra(!1, !0)
    }
      , kp = {
        get: ra(!0, !1)
    }
      , tu = new WeakMap
      , nu = new WeakMap
      , iu = new WeakMap
      , Ap = new WeakMap;
    function $p(t) {
        switch (t) {
        case "Object":
        case "Array":
            return 1;
        case "Map":
        case "Set":
        case "WeakMap":
        case "WeakSet":
            return 2;
        default:
            return 0
        }
    }
    function Tp(t) {
        return t.__v_skip || !Object.isExtensible(t) ? 0 : $p(Yd(t))
    }
    function oa(t) {
        return oi(t) ? t : aa(t, !1, _p, yp, tu)
    }
    function Ep(t) {
        return aa(t, !1, mp, vp, nu)
    }
    function su(t) {
        return aa(t, !0, hp, kp, iu)
    }
    function aa(t, e, n, i, s) {
        if (!xe(t) || t.__v_raw && !(e && t.__v_isReactive))
            return t;
        const r = s.get(t);
        if (r)
            return r;
        const o = Tp(t);
        if (o === 0)
            return t;
        const a = new Proxy(t,o === 2 ? i : n);
        return s.set(t, a),
        a
    }
    function Qi(t) {
        return oi(t) ? Qi(t.__v_raw) : !!(t && t.__v_isReactive)
    }
    function oi(t) {
        return !!(t && t.__v_isReadonly)
    }
    function zt(t) {
        return !!(t && t.__v_isShallow)
    }
    function la(t) {
        return t ? !!t.__v_raw : !1
    }
    function de(t) {
        const e = t && t.__v_raw;
        return e ? de(e) : t
    }
    function wp(t) {
        return !pe(t, "__v_skip") && Object.isExtensible(t) && Gc(t, "__v_skip", !0),
        t
    }
    const lt = t => xe(t) ? oa(t) : t
      , po = t => xe(t) ? su(t) : t;
    function qe(t) {
        return t ? t.__v_isRef === !0 : !1
    }
    function Me(t) {
        return ou(t, !1)
    }
    function ru(t) {
        return ou(t, !0)
    }
    function ou(t, e) {
        return qe(t) ? t : new Sp(t,e)
    }
    class Sp {
        constructor(e, n) {
            this.dep = new ia,
            this.__v_isRef = !0,
            this.__v_isShallow = !1,
            this._rawValue = n ? e : de(e),
            this._value = n ? e : lt(e),
            this.__v_isShallow = n
        }
        get value() {
            return this.dep.track(),
            this._value
        }
        set value(e) {
            const n = this._rawValue
              , i = this.__v_isShallow || zt(e) || oi(e);
            e = i ? e : de(e),
            wn(e, n) && (this._rawValue = e,
            this._value = i ? e : lt(e),
            this.dep.trigger())
        }
    }
    function hi(t) {
        return qe(t) ? t.value : t
    }
    const Op = {
        get: (t, e, n) => e === "__v_raw" ? t : hi(Reflect.get(t, e, n)),
        set: (t, e, n, i) => {
            const s = t[e];
            return qe(s) && !qe(n) ? (s.value = n,
            !0) : Reflect.set(t, e, n, i)
        }
    };
    function au(t) {
        return Qi(t) ? t : new Proxy(t,Op)
    }
    class Pp {
        constructor(e, n, i) {
            this.fn = e,
            this.setter = n,
            this._value = void 0,
            this.dep = new ia(this),
            this.__v_isRef = !0,
            this.deps = void 0,
            this.depsTail = void 0,
            this.flags = 16,
            this.globalVersion = as - 1,
            this.next = void 0,
            this.effect = this,
            this.__v_isReadonly = !n,
            this.isSSR = i
        }
        notify() {
            if (this.flags |= 16,
            !(this.flags & 8) && ke !== this)
                return Kc(this, !0),
                !0
        }
        get value() {
            const e = this.dep.track();
            return Yc(this),
            e && (e.version = this.dep.version),
            this._value
        }
        set value(e) {
            this.setter && this.setter(e)
        }
    }
    function xp(t, e, n=!1) {
        let i, s;
        return re(t) ? i = t : (i = t.get,
        s = t.set),
        new Pp(i,s,n)
    }
    const Ls = {}
      , Ys = new WeakMap;
    let Xn;
    function Ip(t, e=!1, n=Xn) {
        if (n) {
            let i = Ys.get(n);
            i || Ys.set(n, i = []),
            i.push(t)
        }
    }
    function Cp(t, e, n=Ae) {
        const {immediate: i, deep: s, once: r, scheduler: o, augmentJob: a, call: l} = n
          , c = y => s ? y : zt(y) || s === !1 || s === 0 ? An(y, 1) : An(y);
        let u, f, d, _, m = !1, p = !1;
        if (qe(t) ? (f = () => t.value,
        m = zt(t)) : Qi(t) ? (f = () => c(t),
        m = !0) : ie(t) ? (p = !0,
        m = t.some(y => Qi(y) || zt(y)),
        f = () => t.map(y => {
            if (qe(y))
                return y.value;
            if (Qi(y))
                return c(y);
            if (re(y))
                return l ? l(y, 2) : y()
        }
        )) : re(t) ? e ? f = l ? () => l(t, 2) : t : f = () => {
            if (d) {
                Nn();
                try {
                    d()
                } finally {
                    Dn()
                }
            }
            const y = Xn;
            Xn = u;
            try {
                return l ? l(t, 3, [_]) : t(_)
            } finally {
                Xn = y
            }
        }
        : f = en,
        e && s) {
            const y = f
              , A = s === !0 ? 1 / 0 : s;
            f = () => An(y(), A)
        }
        const b = op()
          , E = () => {
            u.stop(),
            b && b.active && Qo(b.effects, u)
        }
        ;
        if (r && e) {
            const y = e;
            e = (...A) => {
                y(...A),
                E()
            }
        }
        let S = p ? new Array(t.length).fill(Ls) : Ls;
        const v = y => {
            if (!(!(u.flags & 1) || !u.dirty && !y))
                if (e) {
                    const A = u.run();
                    if (s || m || (p ? A.some( (T, O) => wn(T, S[O])) : wn(A, S))) {
                        d && d();
                        const T = Xn;
                        Xn = u;
                        try {
                            const O = [A, S === Ls ? void 0 : p && S[0] === Ls ? [] : S, _];
                            l ? l(e, 3, O) : e(...O),
                            S = A
                        } finally {
                            Xn = T
                        }
                    }
                } else
                    u.run()
        }
        ;
        return a && a(v),
        u = new Vc(f),
        u.scheduler = o ? () => o(v, !1) : v,
        _ = y => Ip(y, !1, u),
        d = u.onStop = () => {
            const y = Ys.get(u);
            if (y) {
                if (l)
                    l(y, 4);
                else
                    for (const A of y)
                        A();
                Ys.delete(u)
            }
        }
        ,
        e ? i ? v(!0) : S = u.run() : o ? o(v.bind(null, !0), !0) : u.run(),
        E.pause = u.pause.bind(u),
        E.resume = u.resume.bind(u),
        E.stop = E,
        E
    }
    function An(t, e=1 / 0, n) {
        if (e <= 0 || !xe(t) || t.__v_skip || (n = n || new Set,
        n.has(t)))
            return t;
        if (n.add(t),
        e--,
        qe(t))
            An(t.value, e, n);
        else if (ie(t))
            for (let i = 0; i < t.length; i++)
                An(t[i], e, n);
        else if (Lc(t) || Ai(t))
            t.forEach(i => {
                An(i, e, n)
            }
            );
        else if (Mc(t)) {
            for (const i in t)
                An(t[i], e, n);
            for (const i of Object.getOwnPropertySymbols(t))
                Object.prototype.propertyIsEnumerable.call(t, i) && An(t[i], e, n)
        }
        return t
    }
    /**
* @vue/runtime-core v3.5.13
* (c) 2018-present Yuxi (Evan) You and Vue contributors
* @license MIT
**/
    function Ts(t, e, n, i) {
        try {
            return i ? t(...i) : t()
        } catch (s) {
            gr(s, e, n)
        }
    }
    function sn(t, e, n, i) {
        if (re(t)) {
            const s = Ts(t, e, n, i);
            return s && Nc(s) && s.catch(r => {
                gr(r, e, n)
            }
            ),
            s
        }
        if (ie(t)) {
            const s = [];
            for (let r = 0; r < t.length; r++)
                s.push(sn(t[r], e, n, i));
            return s
        }
    }
    function gr(t, e, n, i=!0) {
        const s = e ? e.vnode : null
          , {errorHandler: r, throwUnhandledErrorInProduction: o} = e && e.appContext.config || Ae;
        if (e) {
            let a = e.parent;
            const l = e.proxy
              , c = "https://vuejs.org/error-reference/#runtime-".concat(n);
            for (; a; ) {
                const u = a.ec;
                if (u) {
                    for (let f = 0; f < u.length; f++)
                        if (u[f](t, l, c) === !1)
                            return
                }
                a = a.parent
            }
            if (r) {
                Nn(),
                Ts(r, null, 10, [t, l, c]),
                Dn();
                return
            }
        }
        Rp(t, n, s, i, o)
    }
    function Rp(t, e, n, i=!0, s=!1) {
        if (s)
            throw t;
        console.error(t)
    }
    const ct = [];
    let Wt = -1;
    const $i = [];
    let vn = null
      , mi = 0;
    const lu = Promise.resolve();
    let Xs = null;
    function cu(t) {
        const e = Xs || lu;
        return t ? e.then(this ? t.bind(this) : t) : e
    }
    function Lp(t) {
        let e = Wt + 1
          , n = ct.length;
        for (; e < n; ) {
            const i = e + n >>> 1
              , s = ct[i]
              , r = cs(s);
            r < t || r === t && s.flags & 2 ? e = i + 1 : n = i
        }
        return e
    }
    function ca(t) {
        if (!(t.flags & 1)) {
            const e = cs(t)
              , n = ct[ct.length - 1];
            !n || !(t.flags & 2) && e >= cs(n) ? ct.push(t) : ct.splice(Lp(e), 0, t),
            t.flags |= 1,
            uu()
        }
    }
    function uu() {
        Xs || (Xs = lu.then(du))
    }
    function Np(t) {
        ie(t) ? $i.push(...t) : vn && t.id === -1 ? vn.splice(mi + 1, 0, t) : t.flags & 1 || ($i.push(t),
        t.flags |= 1),
        uu()
    }
    function Ja(t, e, n=Wt + 1) {
        for (; n < ct.length; n++) {
            const i = ct[n];
            if (i && i.flags & 2) {
                if (t && i.id !== t.uid)
                    continue;
                ct.splice(n, 1),
                n--,
                i.flags & 4 && (i.flags &= -2),
                i(),
                i.flags & 4 || (i.flags &= -2)
            }
        }
    }
    function fu(t) {
        if ($i.length) {
            const e = [...new Set($i)].sort( (n, i) => cs(n) - cs(i));
            if ($i.length = 0,
            vn) {
                vn.push(...e);
                return
            }
            for (vn = e,
            mi = 0; mi < vn.length; mi++) {
                const n = vn[mi];
                n.flags & 4 && (n.flags &= -2),
                n.flags & 8 || n(),
                n.flags &= -2
            }
            vn = null,
            mi = 0
        }
    }
    const cs = t => t.id == null ? t.flags & 2 ? -1 : 1 / 0 : t.id;
    function du(t) {
        try {
            for (Wt = 0; Wt < ct.length; Wt++) {
                const e = ct[Wt];
                e && !(e.flags & 8) && (e.flags & 4 && (e.flags &= -2),
                Ts(e, e.i, e.i ? 15 : 14),
                e.flags & 4 || (e.flags &= -2))
            }
        } finally {
            for (; Wt < ct.length; Wt++) {
                const e = ct[Wt];
                e && (e.flags &= -2)
            }
            Wt = -1,
            ct.length = 0,
            fu(),
            Xs = null,
            (ct.length || $i.length) && du()
        }
    }
    let Gt = null
      , pu = null;
    function Js(t) {
        const e = Gt;
        return Gt = t,
        pu = t && t.type.__scopeId || null,
        e
    }
    function Dp(t, e=Gt, n) {
        if (!e || t._n)
            return t;
        const i = (...s) => {
            i._d && ol(-1);
            const r = Js(e);
            let o;
            try {
                o = t(...s)
            } finally {
                Js(r),
                i._d && ol(1)
            }
            return o
        }
        ;
        return i._n = !0,
        i._c = !0,
        i._d = !0,
        i
    }
    function Vn(t, e, n, i) {
        const s = t.dirs
          , r = e && e.dirs;
        for (let o = 0; o < s.length; o++) {
            const a = s[o];
            r && (a.oldValue = r[o].value);
            let l = a.dir[i];
            l && (Nn(),
            sn(l, n, 8, [t.el, a, t, e]),
            Dn())
        }
    }
    const Mp = Symbol("_vte")
      , Fp = t => t.__isTeleport;
    function ua(t, e) {
        t.shapeFlag & 6 && t.component ? (t.transition = e,
        ua(t.component.subTree, e)) : t.shapeFlag & 128 ? (t.ssContent.transition = e.clone(t.ssContent),
        t.ssFallback.transition = e.clone(t.ssFallback)) : t.transition = e
    }
    /*! #__NO_SIDE_EFFECTS__ */
    function fa(t, e) {
        return re(t) ? it({
            name: t.name
        }, e, {
            setup: t
        }) : t
    }
    function _u(t) {
        t.ids = [t.ids[0] + t.ids[2]++ + "-", 0, 0]
    }
    function Qs(t, e, n, i, s=!1) {
        if (ie(t)) {
            t.forEach( (m, p) => Qs(m, e && (ie(e) ? e[p] : e), n, i, s));
            return
        }
        if (Zi(i) && !s) {
            i.shapeFlag & 512 && i.type.__asyncResolved && i.component.subTree.component && Qs(t, e, n, i.component.subTree);
            return
        }
        const r = i.shapeFlag & 4 ? ma(i.component) : i.el
          , o = s ? null : r
          , {i: a, r: l} = t
          , c = e && e.r
          , u = a.refs === Ae ? a.refs = {} : a.refs
          , f = a.setupState
          , d = de(f)
          , _ = f === Ae ? () => !1 : m => pe(d, m);
        if (c != null && c !== l && (Be(c) ? (u[c] = null,
        _(c) && (f[c] = null)) : qe(c) && (c.value = null)),
        re(l))
            Ts(l, a, 12, [o, u]);
        else {
            const m = Be(l)
              , p = qe(l);
            if (m || p) {
                const b = () => {
                    if (t.f) {
                        const E = m ? _(l) ? f[l] : u[l] : l.value;
                        s ? ie(E) && Qo(E, r) : ie(E) ? E.includes(r) || E.push(r) : m ? (u[l] = [r],
                        _(l) && (f[l] = u[l])) : (l.value = [r],
                        t.k && (u[t.k] = l.value))
                    } else
                        m ? (u[l] = o,
                        _(l) && (f[l] = o)) : p && (l.value = o,
                        t.k && (u[t.k] = o))
                }
                ;
                o ? (b.id = -1,
                $t(b, n)) : b()
            }
        }
    }
    mr().requestIdleCallback;
    mr().cancelIdleCallback;
    const Zi = t => !!t.type.__asyncLoader
      , hu = t => t.type.__isKeepAlive;
    function Gp(t, e) {
        mu(t, "a", e)
    }
    function Up(t, e) {
        mu(t, "da", e)
    }
    function mu(t, e, n=Qe) {
        const i = t.__wdc || (t.__wdc = () => {
            let s = n;
            for (; s; ) {
                if (s.isDeactivated)
                    return;
                s = s.parent
            }
            return t()
        }
        );
        if (br(e, i, n),
        n) {
            let s = n.parent;
            for (; s && s.parent; )
                hu(s.parent.vnode) && jp(i, e, n, s),
                s = s.parent
        }
    }
    function jp(t, e, n, i) {
        const s = br(e, t, i, !0);
        pa( () => {
            Qo(i[e], s)
        }
        , n)
    }
    function br(t, e, n=Qe, i=!1) {
        if (n) {
            const s = n[t] || (n[t] = [])
              , r = e.__weh || (e.__weh = (...o) => {
                Nn();
                const a = ws(n)
                  , l = sn(e, n, t, o);
                return a(),
                Dn(),
                l
            }
            );
            return i ? s.unshift(r) : s.push(r),
            r
        }
    }
    const bn = t => (e, n=Qe) => {
        (!ds || t === "sp") && br(t, (...i) => e(...i), n)
    }
      , gu = bn("bm")
      , da = bn("m")
      , Bp = bn("bu")
      , zp = bn("u")
      , Vp = bn("bum")
      , pa = bn("um")
      , Hp = bn("sp")
      , Kp = bn("rtg")
      , qp = bn("rtc");
    function Wp(t, e=Qe) {
        br("ec", t, e)
    }
    const Yp = Symbol.for("v-ndc")
      , _o = t => t ? Gu(t) ? ma(t) : _o(t.parent) : null
      , es = it(Object.create(null), {
        $: t => t,
        $el: t => t.vnode.el,
        $data: t => t.data,
        $props: t => t.props,
        $attrs: t => t.attrs,
        $slots: t => t.slots,
        $refs: t => t.refs,
        $parent: t => _o(t.parent),
        $root: t => _o(t.root),
        $host: t => t.ce,
        $emit: t => t.emit,
        $options: t => yu(t),
        $forceUpdate: t => t.f || (t.f = () => {
            ca(t.update)
        }
        ),
        $nextTick: t => t.n || (t.n = cu.bind(t.proxy)),
        $watch: t => g_.bind(t)
    })
      , zr = (t, e) => t !== Ae && !t.__isScriptSetup && pe(t, e)
      , Xp = {
        get({_: t}, e) {
            if (e === "__v_skip")
                return !0;
            const {ctx: n, setupState: i, data: s, props: r, accessCache: o, type: a, appContext: l} = t;
            let c;
            if (e[0] !== "$") {
                const _ = o[e];
                if (_ !== void 0)
                    switch (_) {
                    case 1:
                        return i[e];
                    case 2:
                        return s[e];
                    case 4:
                        return n[e];
                    case 3:
                        return r[e]
                    }
                else {
                    if (zr(i, e))
                        return o[e] = 1,
                        i[e];
                    if (s !== Ae && pe(s, e))
                        return o[e] = 2,
                        s[e];
                    if ((c = t.propsOptions[0]) && pe(c, e))
                        return o[e] = 3,
                        r[e];
                    if (n !== Ae && pe(n, e))
                        return o[e] = 4,
                        n[e];
                    ho && (o[e] = 0)
                }
            }
            const u = es[e];
            let f, d;
            if (u)
                return e === "$attrs" && Je(t.attrs, "get", ""),
                u(t);
            if ((f = a.__cssModules) && (f = f[e]))
                return f;
            if (n !== Ae && pe(n, e))
                return o[e] = 4,
                n[e];
            if (d = l.config.globalProperties,
            pe(d, e))
                return d[e]
        },
        set({_: t}, e, n) {
            const {data: i, setupState: s, ctx: r} = t;
            return zr(s, e) ? (s[e] = n,
            !0) : i !== Ae && pe(i, e) ? (i[e] = n,
            !0) : pe(t.props, e) || e[0] === "$" && e.slice(1)in t ? !1 : (r[e] = n,
            !0)
        },
        has({_: {data: t, setupState: e, accessCache: n, ctx: i, appContext: s, propsOptions: r}}, o) {
            let a;
            return !!n[o] || t !== Ae && pe(t, o) || zr(e, o) || (a = r[0]) && pe(a, o) || pe(i, o) || pe(es, o) || pe(s.config.globalProperties, o)
        },
        defineProperty(t, e, n) {
            return n.get != null ? t._.accessCache[e] = 0 : pe(n, "value") && this.set(t, e, n.value, null),
            Reflect.defineProperty(t, e, n)
        }
    };
    function Qa(t) {
        return ie(t) ? t.reduce( (e, n) => (e[n] = null,
        e), {}) : t
    }
    let ho = !0;
    function Jp(t) {
        const e = yu(t)
          , n = t.proxy
          , i = t.ctx;
        ho = !1,
        e.beforeCreate && Za(e.beforeCreate, t, "bc");
        const {data: s, computed: r, methods: o, watch: a, provide: l, inject: c, created: u, beforeMount: f, mounted: d, beforeUpdate: _, updated: m, activated: p, deactivated: b, beforeDestroy: E, beforeUnmount: S, destroyed: v, unmounted: y, render: A, renderTracked: T, renderTriggered: O, errorCaptured: C, serverPrefetch: P, expose: W, inheritAttrs: X, components: q, directives: Q, filters: _e} = e;
        if (c && Qp(c, i, null),
        o)
            for (const J in o) {
                const te = o[J];
                re(te) && (i[J] = te.bind(n))
            }
        if (s) {
            const J = s.call(n, n);
            xe(J) && (t.data = oa(J))
        }
        if (ho = !0,
        r)
            for (const J in r) {
                const te = r[J]
                  , Ie = re(te) ? te.bind(n, n) : re(te.get) ? te.get.bind(n, n) : en
                  , ze = !re(te) && re(te.set) ? te.set.bind(n) : en
                  , he = Ft({
                    get: Ie,
                    set: ze
                });
                Object.defineProperty(i, J, {
                    enumerable: !0,
                    configurable: !0,
                    get: () => he.value,
                    set: me => he.value = me
                })
            }
        if (a)
            for (const J in a)
                bu(a[J], i, n, J);
        if (l) {
            const J = re(l) ? l.call(n) : l;
            Reflect.ownKeys(J).forEach(te => {
                s_(te, J[te])
            }
            )
        }
        u && Za(u, t, "c");
        function Z(J, te) {
            ie(te) ? te.forEach(Ie => J(Ie.bind(n))) : te && J(te.bind(n))
        }
        if (Z(gu, f),
        Z(da, d),
        Z(Bp, _),
        Z(zp, m),
        Z(Gp, p),
        Z(Up, b),
        Z(Wp, C),
        Z(qp, T),
        Z(Kp, O),
        Z(Vp, S),
        Z(pa, y),
        Z(Hp, P),
        ie(W))
            if (W.length) {
                const J = t.exposed || (t.exposed = {});
                W.forEach(te => {
                    Object.defineProperty(J, te, {
                        get: () => n[te],
                        set: Ie => n[te] = Ie
                    })
                }
                )
            } else
                t.exposed || (t.exposed = {});
        A && t.render === en && (t.render = A),
        X != null && (t.inheritAttrs = X),
        q && (t.components = q),
        Q && (t.directives = Q),
        P && _u(t)
    }
    function Qp(t, e, n=en) {
        ie(t) && (t = mo(t));
        for (const i in t) {
            const s = t[i];
            let r;
            xe(s) ? "default"in s ? r = ts(s.from || i, s.default, !0) : r = ts(s.from || i) : r = ts(s),
            qe(r) ? Object.defineProperty(e, i, {
                enumerable: !0,
                configurable: !0,
                get: () => r.value,
                set: o => r.value = o
            }) : e[i] = r
        }
    }
    function Za(t, e, n) {
        sn(ie(t) ? t.map(i => i.bind(e.proxy)) : t.bind(e.proxy), e, n)
    }
    function bu(t, e, n, i) {
        let s = i.includes(".") ? Ru(n, i) : () => n[i];
        if (Be(t)) {
            const r = e[t];
            re(r) && Sn(s, r)
        } else if (re(t))
            Sn(s, t.bind(n));
        else if (xe(t))
            if (ie(t))
                t.forEach(r => bu(r, e, n, i));
            else {
                const r = re(t.handler) ? t.handler.bind(n) : e[t.handler];
                re(r) && Sn(s, r, t)
            }
    }
    function yu(t) {
        const e = t.type
          , {mixins: n, extends: i} = e
          , {mixins: s, optionsCache: r, config: {optionMergeStrategies: o}} = t.appContext
          , a = r.get(e);
        let l;
        return a ? l = a : !s.length && !n && !i ? l = e : (l = {},
        s.length && s.forEach(c => Zs(l, c, o, !0)),
        Zs(l, e, o)),
        xe(e) && r.set(e, l),
        l
    }
    function Zs(t, e, n, i=!1) {
        const {mixins: s, extends: r} = e;
        r && Zs(t, r, n, !0),
        s && s.forEach(o => Zs(t, o, n, !0));
        for (const o in e)
            if (!(i && o === "expose")) {
                const a = Zp[o] || n && n[o];
                t[o] = a ? a(t[o], e[o]) : e[o]
            }
        return t
    }
    const Zp = {
        data: el,
        props: tl,
        emits: tl,
        methods: Ki,
        computed: Ki,
        beforeCreate: rt,
        created: rt,
        beforeMount: rt,
        mounted: rt,
        beforeUpdate: rt,
        updated: rt,
        beforeDestroy: rt,
        beforeUnmount: rt,
        destroyed: rt,
        unmounted: rt,
        activated: rt,
        deactivated: rt,
        errorCaptured: rt,
        serverPrefetch: rt,
        components: Ki,
        directives: Ki,
        watch: t_,
        provide: el,
        inject: e_
    };
    function el(t, e) {
        return e ? t ? function() {
            return it(re(t) ? t.call(this, this) : t, re(e) ? e.call(this, this) : e)
        }
        : e : t
    }
    function e_(t, e) {
        return Ki(mo(t), mo(e))
    }
    function mo(t) {
        if (ie(t)) {
            const e = {};
            for (let n = 0; n < t.length; n++)
                e[t[n]] = t[n];
            return e
        }
        return t
    }
    function rt(t, e) {
        return t ? [...new Set([].concat(t, e))] : e
    }
    function Ki(t, e) {
        return t ? it(Object.create(null), t, e) : e
    }
    function tl(t, e) {
        return t ? ie(t) && ie(e) ? [...new Set([...t, ...e])] : it(Object.create(null), Qa(t), Qa(e != null ? e : {})) : e
    }
    function t_(t, e) {
        if (!t)
            return e;
        if (!e)
            return t;
        const n = it(Object.create(null), t);
        for (const i in e)
            n[i] = rt(t[i], e[i]);
        return n
    }
    function vu() {
        return {
            app: null,
            config: {
                isNativeTag: qd,
                performance: !1,
                globalProperties: {},
                optionMergeStrategies: {},
                errorHandler: void 0,
                warnHandler: void 0,
                compilerOptions: {}
            },
            mixins: [],
            components: {},
            directives: {},
            provides: Object.create(null),
            optionsCache: new WeakMap,
            propsCache: new WeakMap,
            emitsCache: new WeakMap
        }
    }
    let n_ = 0;
    function i_(t, e) {
        return function(i, s=null) {
            re(i) || (i = it({}, i)),
            s != null && !xe(s) && (s = null);
            const r = vu()
              , o = new WeakSet
              , a = [];
            let l = !1;
            const c = r.app = {
                _uid: n_++,
                _component: i,
                _props: s,
                _container: null,
                _context: r,
                _instance: null,
                version: G_,
                get config() {
                    return r.config
                },
                set config(u) {},
                use(u, ...f) {
                    return o.has(u) || (u && re(u.install) ? (o.add(u),
                    u.install(c, ...f)) : re(u) && (o.add(u),
                    u(c, ...f))),
                    c
                },
                mixin(u) {
                    return r.mixins.includes(u) || r.mixins.push(u),
                    c
                },
                component(u, f) {
                    return f ? (r.components[u] = f,
                    c) : r.components[u]
                },
                directive(u, f) {
                    return f ? (r.directives[u] = f,
                    c) : r.directives[u]
                },
                mount(u, f, d) {
                    if (!l) {
                        const _ = c._ceVNode || ft(i, s);
                        return _.appContext = r,
                        d === !0 ? d = "svg" : d === !1 && (d = void 0),
                        t(_, u, d),
                        l = !0,
                        c._container = u,
                        u.__vue_app__ = c,
                        ma(_.component)
                    }
                },
                onUnmount(u) {
                    a.push(u)
                },
                unmount() {
                    l && (sn(a, c._instance, 16),
                    t(null, c._container),
                    delete c._container.__vue_app__)
                },
                provide(u, f) {
                    return r.provides[u] = f,
                    c
                },
                runWithContext(u) {
                    const f = Ti;
                    Ti = c;
                    try {
                        return u()
                    } finally {
                        Ti = f
                    }
                }
            };
            return c
        }
    }
    let Ti = null;
    function s_(t, e) {
        if (Qe) {
            let n = Qe.provides;
            const i = Qe.parent && Qe.parent.provides;
            i === n && (n = Qe.provides = Object.create(i)),
            n[t] = e
        }
    }
    function ts(t, e, n=!1) {
        const i = Qe || Gt;
        if (i || Ti) {
            const s = Ti ? Ti._context.provides : i ? i.parent == null ? i.vnode.appContext && i.vnode.appContext.provides : i.parent.provides : void 0;
            if (s && t in s)
                return s[t];
            if (arguments.length > 1)
                return n && re(e) ? e.call(i && i.proxy) : e
        }
    }
    const ku = {}
      , Au = () => Object.create(ku)
      , $u = t => Object.getPrototypeOf(t) === ku;
    function r_(t, e, n, i=!1) {
        const s = {}
          , r = Au();
        t.propsDefaults = Object.create(null),
        Tu(t, e, s, r);
        for (const o in t.propsOptions[0])
            o in s || (s[o] = void 0);
        n ? t.props = i ? s : Ep(s) : t.type.props ? t.props = s : t.props = r,
        t.attrs = r
    }
    function o_(t, e, n, i) {
        const {props: s, attrs: r, vnode: {patchFlag: o}} = t
          , a = de(s)
          , [l] = t.propsOptions;
        let c = !1;
        if ((i || o > 0) && !(o & 16)) {
            if (o & 8) {
                const u = t.vnode.dynamicProps;
                for (let f = 0; f < u.length; f++) {
                    let d = u[f];
                    if (yr(t.emitsOptions, d))
                        continue;
                    const _ = e[d];
                    if (l)
                        if (pe(r, d))
                            _ !== r[d] && (r[d] = _,
                            c = !0);
                        else {
                            const m = xn(d);
                            s[m] = go(l, a, m, _, t, !1)
                        }
                    else
                        _ !== r[d] && (r[d] = _,
                        c = !0)
                }
            }
        } else {
            Tu(t, e, s, r) && (c = !0);
            let u;
            for (const f in a)
                (!e || !pe(e, f) && ((u = ui(f)) === f || !pe(e, u))) && (l ? n && (n[f] !== void 0 || n[u] !== void 0) && (s[f] = go(l, a, f, void 0, t, !0)) : delete s[f]);
            if (r !== a)
                for (const f in r)
                    (!e || !pe(e, f)) && (delete r[f],
                    c = !0)
        }
        c && _n(t.attrs, "set", "")
    }
    function Tu(t, e, n, i) {
        const [s,r] = t.propsOptions;
        let o = !1, a;
        if (e)
            for (let l in e) {
                if (Yi(l))
                    continue;
                const c = e[l];
                let u;
                s && pe(s, u = xn(l)) ? !r || !r.includes(u) ? n[u] = c : (a || (a = {}))[u] = c : yr(t.emitsOptions, l) || (!(l in i) || c !== i[l]) && (i[l] = c,
                o = !0)
            }
        if (r) {
            const l = de(n)
              , c = a || Ae;
            for (let u = 0; u < r.length; u++) {
                const f = r[u];
                n[f] = go(s, l, f, c[f], t, !pe(c, f))
            }
        }
        return o
    }
    function go(t, e, n, i, s, r) {
        const o = t[n];
        if (o != null) {
            const a = pe(o, "default");
            if (a && i === void 0) {
                const l = o.default;
                if (o.type !== Function && !o.skipFactory && re(l)) {
                    const {propsDefaults: c} = s;
                    if (n in c)
                        i = c[n];
                    else {
                        const u = ws(s);
                        i = c[n] = l.call(null, e),
                        u()
                    }
                } else
                    i = l;
                s.ce && s.ce._setProp(n, i)
            }
            o[0] && (r && !a ? i = !1 : o[1] && (i === "" || i === ui(n)) && (i = !0))
        }
        return i
    }
    const a_ = new WeakMap;
    function Eu(t, e, n=!1) {
        const i = n ? a_ : e.propsCache
          , s = i.get(t);
        if (s)
            return s;
        const r = t.props
          , o = {}
          , a = [];
        let l = !1;
        if (!re(t)) {
            const u = f => {
                l = !0;
                const [d,_] = Eu(f, e, !0);
                it(o, d),
                _ && a.push(..._)
            }
            ;
            !n && e.mixins.length && e.mixins.forEach(u),
            t.extends && u(t.extends),
            t.mixins && t.mixins.forEach(u)
        }
        if (!r && !l)
            return xe(t) && i.set(t, ki),
            ki;
        if (ie(r))
            for (let u = 0; u < r.length; u++) {
                const f = xn(r[u]);
                nl(f) && (o[f] = Ae)
            }
        else if (r)
            for (const u in r) {
                const f = xn(u);
                if (nl(f)) {
                    const d = r[u]
                      , _ = o[f] = ie(d) || re(d) ? {
                        type: d
                    } : it({}, d)
                      , m = _.type;
                    let p = !1
                      , b = !0;
                    if (ie(m))
                        for (let E = 0; E < m.length; ++E) {
                            const S = m[E]
                              , v = re(S) && S.name;
                            if (v === "Boolean") {
                                p = !0;
                                break
                            } else
                                v === "String" && (b = !1)
                        }
                    else
                        p = re(m) && m.name === "Boolean";
                    _[0] = p,
                    _[1] = b,
                    (p || pe(_, "default")) && a.push(f)
                }
            }
        const c = [o, a];
        return xe(t) && i.set(t, c),
        c
    }
    function nl(t) {
        return t[0] !== "$" && !Yi(t)
    }
    const wu = t => t[0] === "_" || t === "$stable"
      , _a = t => ie(t) ? t.map(Xt) : [Xt(t)]
      , l_ = (t, e, n) => {
        if (e._n)
            return e;
        const i = Dp( (...s) => _a(e(...s)), n);
        return i._c = !1,
        i
    }
      , Su = (t, e, n) => {
        const i = t._ctx;
        for (const s in t) {
            if (wu(s))
                continue;
            const r = t[s];
            if (re(r))
                e[s] = l_(s, r, i);
            else if (r != null) {
                const o = _a(r);
                e[s] = () => o
            }
        }
    }
      , Ou = (t, e) => {
        const n = _a(e);
        t.slots.default = () => n
    }
      , Pu = (t, e, n) => {
        for (const i in e)
            (n || i !== "_") && (t[i] = e[i])
    }
      , c_ = (t, e, n) => {
        const i = t.slots = Au();
        if (t.vnode.shapeFlag & 32) {
            const s = e._;
            s ? (Pu(i, e, n),
            n && Gc(i, "_", s, !0)) : Su(e, i)
        } else
            e && Ou(t, e)
    }
      , u_ = (t, e, n) => {
        const {vnode: i, slots: s} = t;
        let r = !0
          , o = Ae;
        if (i.shapeFlag & 32) {
            const a = e._;
            a ? n && a === 1 ? r = !1 : Pu(s, e, n) : (r = !e.$stable,
            Su(e, s)),
            o = e
        } else
            e && (Ou(t, e),
            o = {
                default: 1
            });
        if (r)
            for (const a in s)
                !wu(a) && o[a] == null && delete s[a]
    }
      , $t = T_;
    function f_(t) {
        return d_(t)
    }
    function d_(t, e) {
        const n = mr();
        n.__VUE__ = !0;
        const {insert: i, remove: s, patchProp: r, createElement: o, createText: a, createComment: l, setText: c, setElementText: u, parentNode: f, nextSibling: d, setScopeId: _=en, insertStaticContent: m} = t
          , p = (k, w, $, N=null, M=null, F=null, z=void 0, B=null, h=!!w.dynamicChildren) => {
            if (k === w)
                return;
            k && !Bi(k, w) && (N = be(k),
            me(k, M, F, !0),
            k = null),
            w.patchFlag === -2 && (h = !1,
            w.dynamicChildren = null);
            const {type: g, ref: I, shapeFlag: L} = w;
            switch (g) {
            case Es:
                b(k, w, $, N);
                break;
            case ai:
                E(k, w, $, N);
                break;
            case Fs:
                k == null && S(w, $, N, z);
                break;
            case Mt:
                q(k, w, $, N, M, F, z, B, h);
                break;
            default:
                L & 1 ? A(k, w, $, N, M, F, z, B, h) : L & 6 ? Q(k, w, $, N, M, F, z, B, h) : (L & 64 || L & 128) && g.process(k, w, $, N, M, F, z, B, h, Re)
            }
            I != null && M && Qs(I, k && k.ref, F, w || k, !w)
        }
          , b = (k, w, $, N) => {
            if (k == null)
                i(w.el = a(w.children), $, N);
            else {
                const M = w.el = k.el;
                w.children !== k.children && c(M, w.children)
            }
        }
          , E = (k, w, $, N) => {
            k == null ? i(w.el = l(w.children || ""), $, N) : w.el = k.el
        }
          , S = (k, w, $, N) => {
            [k.el,k.anchor] = m(k.children, w, $, N, k.el, k.anchor)
        }
          , v = ({el: k, anchor: w}, $, N) => {
            let M;
            for (; k && k !== w; )
                M = d(k),
                i(k, $, N),
                k = M;
            i(w, $, N)
        }
          , y = ({el: k, anchor: w}) => {
            let $;
            for (; k && k !== w; )
                $ = d(k),
                s(k),
                k = $;
            s(w)
        }
          , A = (k, w, $, N, M, F, z, B, h) => {
            w.type === "svg" ? z = "svg" : w.type === "math" && (z = "mathml"),
            k == null ? T(w, $, N, M, F, z, B, h) : P(k, w, M, F, z, B, h)
        }
          , T = (k, w, $, N, M, F, z, B) => {
            let h, g;
            const {props: I, shapeFlag: L, transition: V, dirs: U} = k;
            if (h = k.el = o(k.type, F, I && I.is, I),
            L & 8 ? u(h, k.children) : L & 16 && C(k.children, h, null, N, M, Vr(k, F), z, B),
            U && Vn(k, null, N, "created"),
            O(h, k, k.scopeId, z, N),
            I) {
                for (const D in I)
                    D !== "value" && !Yi(D) && r(h, D, null, I[D], F, N);
                "value"in I && r(h, "value", null, I.value, F),
                (g = I.onVnodeBeforeMount) && Kt(g, N, k)
            }
            U && Vn(k, null, N, "beforeMount");
            const x = p_(M, V);
            x && V.beforeEnter(h),
            i(h, w, $),
            ((g = I && I.onVnodeMounted) || x || U) && $t( () => {
                g && Kt(g, N, k),
                x && V.enter(h),
                U && Vn(k, null, N, "mounted")
            }
            , M)
        }
          , O = (k, w, $, N, M) => {
            if ($ && _(k, $),
            N)
                for (let F = 0; F < N.length; F++)
                    _(k, N[F]);
            if (M) {
                let F = M.subTree;
                if (w === F || Nu(F.type) && (F.ssContent === w || F.ssFallback === w)) {
                    const z = M.vnode;
                    O(k, z, z.scopeId, z.slotScopeIds, M.parent)
                }
            }
        }
          , C = (k, w, $, N, M, F, z, B, h=0) => {
            for (let g = h; g < k.length; g++) {
                const I = k[g] = B ? kn(k[g]) : Xt(k[g]);
                p(null, I, w, $, N, M, F, z, B)
            }
        }
          , P = (k, w, $, N, M, F, z) => {
            const B = w.el = k.el;
            let {patchFlag: h, dynamicChildren: g, dirs: I} = w;
            h |= k.patchFlag & 16;
            const L = k.props || Ae
              , V = w.props || Ae;
            let U;
            if ($ && Hn($, !1),
            (U = V.onVnodeBeforeUpdate) && Kt(U, $, w, k),
            I && Vn(w, k, $, "beforeUpdate"),
            $ && Hn($, !0),
            (L.innerHTML && V.innerHTML == null || L.textContent && V.textContent == null) && u(B, ""),
            g ? W(k.dynamicChildren, g, B, $, N, Vr(w, M), F) : z || te(k, w, B, null, $, N, Vr(w, M), F, !1),
            h > 0) {
                if (h & 16)
                    X(B, L, V, $, M);
                else if (h & 2 && L.class !== V.class && r(B, "class", null, V.class, M),
                h & 4 && r(B, "style", L.style, V.style, M),
                h & 8) {
                    const x = w.dynamicProps;
                    for (let D = 0; D < x.length; D++) {
                        const Y = x[D]
                          , ae = L[Y]
                          , we = V[Y];
                        (we !== ae || Y === "value") && r(B, Y, ae, we, M, $)
                    }
                }
                h & 1 && k.children !== w.children && u(B, w.children)
            } else
                !z && g == null && X(B, L, V, $, M);
            ((U = V.onVnodeUpdated) || I) && $t( () => {
                U && Kt(U, $, w, k),
                I && Vn(w, k, $, "updated")
            }
            , N)
        }
          , W = (k, w, $, N, M, F, z) => {
            for (let B = 0; B < w.length; B++) {
                const h = k[B]
                  , g = w[B]
                  , I = h.el && (h.type === Mt || !Bi(h, g) || h.shapeFlag & 70) ? f(h.el) : $;
                p(h, g, I, null, N, M, F, z, !0)
            }
        }
          , X = (k, w, $, N, M) => {
            if (w !== $) {
                if (w !== Ae)
                    for (const F in w)
                        !Yi(F) && !(F in $) && r(k, F, w[F], null, M, N);
                for (const F in $) {
                    if (Yi(F))
                        continue;
                    const z = $[F]
                      , B = w[F];
                    z !== B && F !== "value" && r(k, F, B, z, M, N)
                }
                "value"in $ && r(k, "value", w.value, $.value, M)
            }
        }
          , q = (k, w, $, N, M, F, z, B, h) => {
            const g = w.el = k ? k.el : a("")
              , I = w.anchor = k ? k.anchor : a("");
            let {patchFlag: L, dynamicChildren: V, slotScopeIds: U} = w;
            U && (B = B ? B.concat(U) : U),
            k == null ? (i(g, $, N),
            i(I, $, N),
            C(w.children || [], $, I, M, F, z, B, h)) : L > 0 && L & 64 && V && k.dynamicChildren ? (W(k.dynamicChildren, V, $, M, F, z, B),
            (w.key != null || M && w === M.subTree) && xu(k, w, !0)) : te(k, w, $, I, M, F, z, B, h)
        }
          , Q = (k, w, $, N, M, F, z, B, h) => {
            w.slotScopeIds = B,
            k == null ? w.shapeFlag & 512 ? M.ctx.activate(w, $, N, z, h) : _e(w, $, N, M, F, z, h) : ue(k, w, h)
        }
          , _e = (k, w, $, N, M, F, z) => {
            const B = k.component = R_(k, N, M);
            if (hu(k) && (B.ctx.renderer = Re),
            L_(B, !1, z),
            B.asyncDep) {
                if (M && M.registerDep(B, Z, z),
                !k.el) {
                    const h = B.subTree = ft(ai);
                    E(null, h, w, $)
                }
            } else
                Z(B, k, w, $, M, F, z)
        }
          , ue = (k, w, $) => {
            const N = w.component = k.component;
            if (A_(k, w, $))
                if (N.asyncDep && !N.asyncResolved) {
                    J(N, w, $);
                    return
                } else
                    N.next = w,
                    N.update();
            else
                w.el = k.el,
                N.vnode = w
        }
          , Z = (k, w, $, N, M, F, z) => {
            const B = () => {
                if (k.isMounted) {
                    let {next: L, bu: V, u: U, parent: x, vnode: D} = k;
                    {
                        const Ke = Iu(k);
                        if (Ke) {
                            L && (L.el = D.el,
                            J(k, L, z)),
                            Ke.asyncDep.then( () => {
                                k.isUnmounted || B()
                            }
                            );
                            return
                        }
                    }
                    let Y = L, ae;
                    Hn(k, !1),
                    L ? (L.el = D.el,
                    J(k, L, z)) : L = D,
                    V && Fr(V),
                    (ae = L.props && L.props.onVnodeBeforeUpdate) && Kt(ae, x, L, D),
                    Hn(k, !0);
                    const we = sl(k)
                      , st = k.subTree;
                    k.subTree = we,
                    p(st, we, f(st.el), be(st), k, M, F),
                    L.el = we.el,
                    Y === null && $_(k, we.el),
                    U && $t(U, M),
                    (ae = L.props && L.props.onVnodeUpdated) && $t( () => Kt(ae, x, L, D), M)
                } else {
                    let L;
                    const {el: V, props: U} = w
                      , {bm: x, m: D, parent: Y, root: ae, type: we} = k
                      , st = Zi(w);
                    Hn(k, !1),
                    x && Fr(x),
                    !st && (L = U && U.onVnodeBeforeMount) && Kt(L, Y, w),
                    Hn(k, !0);
                    {
                        ae.ce && ae.ce._injectChildStyle(we);
                        const Ke = k.subTree = sl(k);
                        p(null, Ke, $, N, k, M, F),
                        w.el = Ke.el
                    }
                    if (D && $t(D, M),
                    !st && (L = U && U.onVnodeMounted)) {
                        const Ke = w;
                        $t( () => Kt(L, Y, Ke), M)
                    }
                    (w.shapeFlag & 256 || Y && Zi(Y.vnode) && Y.vnode.shapeFlag & 256) && k.a && $t(k.a, M),
                    k.isMounted = !0,
                    w = $ = N = null
                }
            }
            ;
            k.scope.on();
            const h = k.effect = new Vc(B);
            k.scope.off();
            const g = k.update = h.run.bind(h)
              , I = k.job = h.runIfDirty.bind(h);
            I.i = k,
            I.id = k.uid,
            h.scheduler = () => ca(I),
            Hn(k, !0),
            g()
        }
          , J = (k, w, $) => {
            w.component = k;
            const N = k.vnode.props;
            k.vnode = w,
            k.next = null,
            o_(k, w.props, N, $),
            u_(k, w.children, $),
            Nn(),
            Ja(k),
            Dn()
        }
          , te = (k, w, $, N, M, F, z, B, h=!1) => {
            const g = k && k.children
              , I = k ? k.shapeFlag : 0
              , L = w.children
              , {patchFlag: V, shapeFlag: U} = w;
            if (V > 0) {
                if (V & 128) {
                    ze(g, L, $, N, M, F, z, B, h);
                    return
                } else if (V & 256) {
                    Ie(g, L, $, N, M, F, z, B, h);
                    return
                }
            }
            U & 8 ? (I & 16 && ge(g, M, F),
            L !== g && u($, L)) : I & 16 ? U & 16 ? ze(g, L, $, N, M, F, z, B, h) : ge(g, M, F, !0) : (I & 8 && u($, ""),
            U & 16 && C(L, $, N, M, F, z, B, h))
        }
          , Ie = (k, w, $, N, M, F, z, B, h) => {
            k = k || ki,
            w = w || ki;
            const g = k.length
              , I = w.length
              , L = Math.min(g, I);
            let V;
            for (V = 0; V < L; V++) {
                const U = w[V] = h ? kn(w[V]) : Xt(w[V]);
                p(k[V], U, $, null, M, F, z, B, h)
            }
            g > I ? ge(k, M, F, !0, !1, L) : C(w, $, N, M, F, z, B, h, L)
        }
          , ze = (k, w, $, N, M, F, z, B, h) => {
            let g = 0;
            const I = w.length;
            let L = k.length - 1
              , V = I - 1;
            for (; g <= L && g <= V; ) {
                const U = k[g]
                  , x = w[g] = h ? kn(w[g]) : Xt(w[g]);
                if (Bi(U, x))
                    p(U, x, $, null, M, F, z, B, h);
                else
                    break;
                g++
            }
            for (; g <= L && g <= V; ) {
                const U = k[L]
                  , x = w[V] = h ? kn(w[V]) : Xt(w[V]);
                if (Bi(U, x))
                    p(U, x, $, null, M, F, z, B, h);
                else
                    break;
                L--,
                V--
            }
            if (g > L) {
                if (g <= V) {
                    const U = V + 1
                      , x = U < I ? w[U].el : N;
                    for (; g <= V; )
                        p(null, w[g] = h ? kn(w[g]) : Xt(w[g]), $, x, M, F, z, B, h),
                        g++
                }
            } else if (g > V)
                for (; g <= L; )
                    me(k[g], M, F, !0),
                    g++;
            else {
                const U = g
                  , x = g
                  , D = new Map;
                for (g = x; g <= V; g++) {
                    const kt = w[g] = h ? kn(w[g]) : Xt(w[g]);
                    kt.key != null && D.set(kt.key, g)
                }
                let Y, ae = 0;
                const we = V - x + 1;
                let st = !1
                  , Ke = 0;
                const jn = new Array(we);
                for (g = 0; g < we; g++)
                    jn[g] = 0;
                for (g = U; g <= L; g++) {
                    const kt = k[g];
                    if (ae >= we) {
                        me(kt, M, F, !0);
                        continue
                    }
                    let Ht;
                    if (kt.key != null)
                        Ht = D.get(kt.key);
                    else
                        for (Y = x; Y <= V; Y++)
                            if (jn[Y - x] === 0 && Bi(kt, w[Y])) {
                                Ht = Y;
                                break
                            }
                    Ht === void 0 ? me(kt, M, F, !0) : (jn[Ht - x] = g + 1,
                    Ht >= Ke ? Ke = Ht : st = !0,
                    p(kt, w[Ht], $, null, M, F, z, B, h),
                    ae++)
                }
                const Lr = st ? __(jn) : ki;
                for (Y = Lr.length - 1,
                g = we - 1; g >= 0; g--) {
                    const kt = x + g
                      , Ht = w[kt]
                      , Va = kt + 1 < I ? w[kt + 1].el : N;
                    jn[g] === 0 ? p(null, Ht, $, Va, M, F, z, B, h) : st && (Y < 0 || g !== Lr[Y] ? he(Ht, $, Va, 2) : Y--)
                }
            }
        }
          , he = (k, w, $, N, M=null) => {
            const {el: F, type: z, transition: B, children: h, shapeFlag: g} = k;
            if (g & 6) {
                he(k.component.subTree, w, $, N);
                return
            }
            if (g & 128) {
                k.suspense.move(w, $, N);
                return
            }
            if (g & 64) {
                z.move(k, w, $, Re);
                return
            }
            if (z === Mt) {
                i(F, w, $);
                for (let L = 0; L < h.length; L++)
                    he(h[L], w, $, N);
                i(k.anchor, w, $);
                return
            }
            if (z === Fs) {
                v(k, w, $);
                return
            }
            if (N !== 2 && g & 1 && B)
                if (N === 0)
                    B.beforeEnter(F),
                    i(F, w, $),
                    $t( () => B.enter(F), M);
                else {
                    const {leave: L, delayLeave: V, afterLeave: U} = B
                      , x = () => i(F, w, $)
                      , D = () => {
                        L(F, () => {
                            x(),
                            U && U()
                        }
                        )
                    }
                    ;
                    V ? V(F, x, D) : D()
                }
            else
                i(F, w, $)
        }
          , me = (k, w, $, N=!1, M=!1) => {
            const {type: F, props: z, ref: B, children: h, dynamicChildren: g, shapeFlag: I, patchFlag: L, dirs: V, cacheIndex: U} = k;
            if (L === -2 && (M = !1),
            B != null && Qs(B, null, $, k, !0),
            U != null && (w.renderCache[U] = void 0),
            I & 256) {
                w.ctx.deactivate(k);
                return
            }
            const x = I & 1 && V
              , D = !Zi(k);
            let Y;
            if (D && (Y = z && z.onVnodeBeforeUnmount) && Kt(Y, w, k),
            I & 6)
                K(k.component, $, N);
            else {
                if (I & 128) {
                    k.suspense.unmount($, N);
                    return
                }
                x && Vn(k, null, w, "beforeUnmount"),
                I & 64 ? k.type.remove(k, w, $, Re, N) : g && !g.hasOnce && (F !== Mt || L > 0 && L & 64) ? ge(g, w, $, !1, !0) : (F === Mt && L & 384 || !M && I & 16) && ge(h, w, $),
                N && vt(k)
            }
            (D && (Y = z && z.onVnodeUnmounted) || x) && $t( () => {
                Y && Kt(Y, w, k),
                x && Vn(k, null, w, "unmounted")
            }
            , $)
        }
          , vt = k => {
            const {type: w, el: $, anchor: N, transition: M} = k;
            if (w === Mt) {
                G($, N);
                return
            }
            if (w === Fs) {
                y(k);
                return
            }
            const F = () => {
                s($),
                M && !M.persisted && M.afterLeave && M.afterLeave()
            }
            ;
            if (k.shapeFlag & 1 && M && !M.persisted) {
                const {leave: z, delayLeave: B} = M
                  , h = () => z($, F);
                B ? B(k.el, F, h) : h()
            } else
                F()
        }
          , G = (k, w) => {
            let $;
            for (; k !== w; )
                $ = d(k),
                s(k),
                k = $;
            s(w)
        }
          , K = (k, w, $) => {
            const {bum: N, scope: M, job: F, subTree: z, um: B, m: h, a: g} = k;
            il(h),
            il(g),
            N && Fr(N),
            M.stop(),
            F && (F.flags |= 8,
            me(z, k, w, $)),
            B && $t(B, w),
            $t( () => {
                k.isUnmounted = !0
            }
            , w),
            w && w.pendingBranch && !w.isUnmounted && k.asyncDep && !k.asyncResolved && k.suspenseId === w.pendingId && (w.deps--,
            w.deps === 0 && w.resolve())
        }
          , ge = (k, w, $, N=!1, M=!1, F=0) => {
            for (let z = F; z < k.length; z++)
                me(k[z], w, $, N, M)
        }
          , be = k => {
            if (k.shapeFlag & 6)
                return be(k.component.subTree);
            if (k.shapeFlag & 128)
                return k.suspense.next();
            const w = d(k.anchor || k.el)
              , $ = w && w[Mp];
            return $ ? d($) : w
        }
        ;
        let Ue = !1;
        const Ce = (k, w, $) => {
            k == null ? w._vnode && me(w._vnode, null, null, !0) : p(w._vnode || null, k, w, null, null, null, $),
            w._vnode = k,
            Ue || (Ue = !0,
            Ja(),
            fu(),
            Ue = !1)
        }
          , Re = {
            p,
            um: me,
            m: he,
            r: vt,
            mt: _e,
            mc: C,
            pc: te,
            pbc: W,
            n: be,
            o: t
        };
        return {
            render: Ce,
            hydrate: void 0,
            createApp: i_(Ce)
        }
    }
    function Vr({type: t, props: e}, n) {
        return n === "svg" && t === "foreignObject" || n === "mathml" && t === "annotation-xml" && e && e.encoding && e.encoding.includes("html") ? void 0 : n
    }
    function Hn({effect: t, job: e}, n) {
        n ? (t.flags |= 32,
        e.flags |= 4) : (t.flags &= -33,
        e.flags &= -5)
    }
    function p_(t, e) {
        return (!t || t && !t.pendingBranch) && e && !e.persisted
    }
    function xu(t, e, n=!1) {
        const i = t.children
          , s = e.children;
        if (ie(i) && ie(s))
            for (let r = 0; r < i.length; r++) {
                const o = i[r];
                let a = s[r];
                a.shapeFlag & 1 && !a.dynamicChildren && ((a.patchFlag <= 0 || a.patchFlag === 32) && (a = s[r] = kn(s[r]),
                a.el = o.el),
                !n && a.patchFlag !== -2 && xu(o, a)),
                a.type === Es && (a.el = o.el)
            }
    }
    function __(t) {
        const e = t.slice()
          , n = [0];
        let i, s, r, o, a;
        const l = t.length;
        for (i = 0; i < l; i++) {
            const c = t[i];
            if (c !== 0) {
                if (s = n[n.length - 1],
                t[s] < c) {
                    e[i] = s,
                    n.push(i);
                    continue
                }
                for (r = 0,
                o = n.length - 1; r < o; )
                    a = r + o >> 1,
                    t[n[a]] < c ? r = a + 1 : o = a;
                c < t[n[r]] && (r > 0 && (e[i] = n[r - 1]),
                n[r] = i)
            }
        }
        for (r = n.length,
        o = n[r - 1]; r-- > 0; )
            n[r] = o,
            o = e[o];
        return n
    }
    function Iu(t) {
        const e = t.subTree.component;
        if (e)
            return e.asyncDep && !e.asyncResolved ? e : Iu(e)
    }
    function il(t) {
        if (t)
            for (let e = 0; e < t.length; e++)
                t[e].flags |= 8
    }
    const h_ = Symbol.for("v-scx")
      , m_ = () => ts(h_);
    function Sn(t, e, n) {
        return Cu(t, e, n)
    }
    function Cu(t, e, n=Ae) {
        const {immediate: i, deep: s, flush: r, once: o} = n
          , a = it({}, n)
          , l = e && i || !e && r !== "post";
        let c;
        if (ds) {
            if (r === "sync") {
                const _ = m_();
                c = _.__watcherHandles || (_.__watcherHandles = [])
            } else if (!l) {
                const _ = () => {}
                ;
                return _.stop = en,
                _.resume = en,
                _.pause = en,
                _
            }
        }
        const u = Qe;
        a.call = (_, m, p) => sn(_, u, m, p);
        let f = !1;
        r === "post" ? a.scheduler = _ => {
            $t(_, u && u.suspense)
        }
        : r !== "sync" && (f = !0,
        a.scheduler = (_, m) => {
            m ? _() : ca(_)
        }
        ),
        a.augmentJob = _ => {
            e && (_.flags |= 4),
            f && (_.flags |= 2,
            u && (_.id = u.uid,
            _.i = u))
        }
        ;
        const d = Cp(t, e, a);
        return ds && (c ? c.push(d) : l && d()),
        d
    }
    function g_(t, e, n) {
        const i = this.proxy
          , s = Be(t) ? t.includes(".") ? Ru(i, t) : () => i[t] : t.bind(i, i);
        let r;
        re(e) ? r = e : (r = e.handler,
        n = e);
        const o = ws(this)
          , a = Cu(s, r.bind(i), n);
        return o(),
        a
    }
    function Ru(t, e) {
        const n = e.split(".");
        return () => {
            let i = t;
            for (let s = 0; s < n.length && i; s++)
                i = i[n[s]];
            return i
        }
    }
    const b_ = (t, e) => e === "modelValue" || e === "model-value" ? t.modelModifiers : t["".concat(e, "Modifiers")] || t["".concat(xn(e), "Modifiers")] || t["".concat(ui(e), "Modifiers")];
    function y_(t, e, ...n) {
        if (t.isUnmounted)
            return;
        const i = t.vnode.props || Ae;
        let s = n;
        const r = e.startsWith("update:")
          , o = r && b_(i, e.slice(7));
        o && (o.trim && (s = n.map(u => Be(u) ? u.trim() : u)),
        o.number && (s = n.map(Qd)));
        let a, l = i[a = Mr(e)] || i[a = Mr(xn(e))];
        !l && r && (l = i[a = Mr(ui(e))]),
        l && sn(l, t, 6, s);
        const c = i[a + "Once"];
        if (c) {
            if (!t.emitted)
                t.emitted = {};
            else if (t.emitted[a])
                return;
            t.emitted[a] = !0,
            sn(c, t, 6, s)
        }
    }
    function Lu(t, e, n=!1) {
        const i = e.emitsCache
          , s = i.get(t);
        if (s !== void 0)
            return s;
        const r = t.emits;
        let o = {}
          , a = !1;
        if (!re(t)) {
            const l = c => {
                const u = Lu(c, e, !0);
                u && (a = !0,
                it(o, u))
            }
            ;
            !n && e.mixins.length && e.mixins.forEach(l),
            t.extends && l(t.extends),
            t.mixins && t.mixins.forEach(l)
        }
        return !r && !a ? (xe(t) && i.set(t, null),
        null) : (ie(r) ? r.forEach(l => o[l] = null) : it(o, r),
        xe(t) && i.set(t, o),
        o)
    }
    function yr(t, e) {
        return !t || !pr(e) ? !1 : (e = e.slice(2).replace(/Once$/, ""),
        pe(t, e[0].toLowerCase() + e.slice(1)) || pe(t, ui(e)) || pe(t, e))
    }
    function sl(t) {
        const {type: e, vnode: n, proxy: i, withProxy: s, propsOptions: [r], slots: o, attrs: a, emit: l, render: c, renderCache: u, props: f, data: d, setupState: _, ctx: m, inheritAttrs: p} = t
          , b = Js(t);
        let E, S;
        try {
            if (n.shapeFlag & 4) {
                const y = s || i
                  , A = y;
                E = Xt(c.call(A, y, u, f, _, d, m)),
                S = a
            } else {
                const y = e;
                E = Xt(y.length > 1 ? y(f, {
                    attrs: a,
                    slots: o,
                    emit: l
                }) : y(f, null)),
                S = e.props ? a : v_(a)
            }
        } catch (y) {
            ns.length = 0,
            gr(y, t, 1),
            E = ft(ai)
        }
        let v = E;
        if (S && p !== !1) {
            const y = Object.keys(S)
              , {shapeFlag: A} = v;
            y.length && A & 7 && (r && y.some(Jo) && (S = k_(S, r)),
            v = Oi(v, S, !1, !0))
        }
        return n.dirs && (v = Oi(v, null, !1, !0),
        v.dirs = v.dirs ? v.dirs.concat(n.dirs) : n.dirs),
        n.transition && ua(v, n.transition),
        E = v,
        Js(b),
        E
    }
    const v_ = t => {
        let e;
        for (const n in t)
            (n === "class" || n === "style" || pr(n)) && ((e || (e = {}))[n] = t[n]);
        return e
    }
      , k_ = (t, e) => {
        const n = {};
        for (const i in t)
            (!Jo(i) || !(i.slice(9)in e)) && (n[i] = t[i]);
        return n
    }
    ;
    function A_(t, e, n) {
        const {props: i, children: s, component: r} = t
          , {props: o, children: a, patchFlag: l} = e
          , c = r.emitsOptions;
        if (e.dirs || e.transition)
            return !0;
        if (n && l >= 0) {
            if (l & 1024)
                return !0;
            if (l & 16)
                return i ? rl(i, o, c) : !!o;
            if (l & 8) {
                const u = e.dynamicProps;
                for (let f = 0; f < u.length; f++) {
                    const d = u[f];
                    if (o[d] !== i[d] && !yr(c, d))
                        return !0
                }
            }
        } else
            return (s || a) && (!a || !a.$stable) ? !0 : i === o ? !1 : i ? o ? rl(i, o, c) : !0 : !!o;
        return !1
    }
    function rl(t, e, n) {
        const i = Object.keys(e);
        if (i.length !== Object.keys(t).length)
            return !0;
        for (let s = 0; s < i.length; s++) {
            const r = i[s];
            if (e[r] !== t[r] && !yr(n, r))
                return !0
        }
        return !1
    }
    function $_({vnode: t, parent: e}, n) {
        for (; e; ) {
            const i = e.subTree;
            if (i.suspense && i.suspense.activeBranch === t && (i.el = t.el),
            i === t)
                (t = e.vnode).el = n,
                e = e.parent;
            else
                break
        }
    }
    const Nu = t => t.__isSuspense;
    function T_(t, e) {
        e && e.pendingBranch ? ie(t) ? e.effects.push(...t) : e.effects.push(t) : Np(t)
    }
    const Mt = Symbol.for("v-fgt")
      , Es = Symbol.for("v-txt")
      , ai = Symbol.for("v-cmt")
      , Fs = Symbol.for("v-stc")
      , ns = [];
    let St = null;
    function ot(t=!1) {
        ns.push(St = t ? null : [])
    }
    function E_() {
        ns.pop(),
        St = ns[ns.length - 1] || null
    }
    let us = 1;
    function ol(t, e=!1) {
        us += t,
        t < 0 && St && e && (St.hasOnce = !0)
    }
    function Du(t) {
        return t.dynamicChildren = us > 0 ? St || ki : null,
        E_(),
        us > 0 && St && St.push(t),
        t
    }
    function dt(t, e, n, i, s, r) {
        return Du(j(t, e, n, i, s, r, !0))
    }
    function w_(t, e, n, i, s) {
        return Du(ft(t, e, n, i, s, !0))
    }
    function er(t) {
        return t ? t.__v_isVNode === !0 : !1
    }
    function Bi(t, e) {
        return t.type === e.type && t.key === e.key
    }
    const Mu = ({key: t}) => t != null ? t : null
      , Gs = ({ref: t, ref_key: e, ref_for: n}) => (typeof t == "number" && (t = "" + t),
    t != null ? Be(t) || qe(t) || re(t) ? {
        i: Gt,
        r: t,
        k: e,
        f: !!n
    } : t : null);
    function j(t, e=null, n=null, i=0, s=null, r=t === Mt ? 0 : 1, o=!1, a=!1) {
        const l = {
            __v_isVNode: !0,
            __v_skip: !0,
            type: t,
            props: e,
            key: e && Mu(e),
            ref: e && Gs(e),
            scopeId: pu,
            slotScopeIds: null,
            children: n,
            component: null,
            suspense: null,
            ssContent: null,
            ssFallback: null,
            dirs: null,
            transition: null,
            el: null,
            anchor: null,
            target: null,
            targetStart: null,
            targetAnchor: null,
            staticCount: 0,
            shapeFlag: r,
            patchFlag: i,
            dynamicProps: s,
            dynamicChildren: null,
            appContext: null,
            ctx: Gt
        };
        return a ? (ha(l, n),
        r & 128 && t.normalize(l)) : n && (l.shapeFlag |= Be(n) ? 8 : 16),
        us > 0 && !o && St && (l.patchFlag > 0 || r & 6) && l.patchFlag !== 32 && St.push(l),
        l
    }
    const ft = S_;
    function S_(t, e=null, n=null, i=0, s=null, r=!1) {
        if ((!t || t === Yp) && (t = ai),
        er(t)) {
            const a = Oi(t, e, !0);
            return n && ha(a, n),
            us > 0 && !r && St && (a.shapeFlag & 6 ? St[St.indexOf(t)] = a : St.push(a)),
            a.patchFlag = -2,
            a
        }
        if (F_(t) && (t = t.__vccOpts),
        e) {
            e = O_(e);
            let {class: a, style: l} = e;
            a && !Be(a) && (e.class = ce(a)),
            xe(l) && (la(l) && !ie(l) && (l = it({}, l)),
            e.style = os(l))
        }
        const o = Be(t) ? 1 : Nu(t) ? 128 : Fp(t) ? 64 : xe(t) ? 4 : re(t) ? 2 : 0;
        return j(t, e, n, i, s, o, r, !0)
    }
    function O_(t) {
        return t ? la(t) || $u(t) ? it({}, t) : t : null
    }
    function Oi(t, e, n=!1, i=!1) {
        const {props: s, ref: r, patchFlag: o, children: a, transition: l} = t
          , c = e ? x_(s || {}, e) : s
          , u = {
            __v_isVNode: !0,
            __v_skip: !0,
            type: t.type,
            props: c,
            key: c && Mu(c),
            ref: e && e.ref ? n && r ? ie(r) ? r.concat(Gs(e)) : [r, Gs(e)] : Gs(e) : r,
            scopeId: t.scopeId,
            slotScopeIds: t.slotScopeIds,
            children: a,
            target: t.target,
            targetStart: t.targetStart,
            targetAnchor: t.targetAnchor,
            staticCount: t.staticCount,
            shapeFlag: t.shapeFlag,
            patchFlag: e && t.type !== Mt ? o === -1 ? 16 : o | 16 : o,
            dynamicProps: t.dynamicProps,
            dynamicChildren: t.dynamicChildren,
            appContext: t.appContext,
            dirs: t.dirs,
            transition: l,
            component: t.component,
            suspense: t.suspense,
            ssContent: t.ssContent && Oi(t.ssContent),
            ssFallback: t.ssFallback && Oi(t.ssFallback),
            el: t.el,
            anchor: t.anchor,
            ctx: t.ctx,
            ce: t.ce
        };
        return l && i && ua(u, l.clone(u)),
        u
    }
    function Fu(t=" ", e=0) {
        return ft(Es, null, t, e)
    }
    function P_(t, e) {
        const n = ft(Fs, null, t);
        return n.staticCount = e,
        n
    }
    function ln(t="", e=!1) {
        return e ? (ot(),
        w_(ai, null, t)) : ft(ai, null, t)
    }
    function Xt(t) {
        return t == null || typeof t == "boolean" ? ft(ai) : ie(t) ? ft(Mt, null, t.slice()) : er(t) ? kn(t) : ft(Es, null, String(t))
    }
    function kn(t) {
        return t.el === null && t.patchFlag !== -1 || t.memo ? t : Oi(t)
    }
    function ha(t, e) {
        let n = 0;
        const {shapeFlag: i} = t;
        if (e == null)
            e = null;
        else if (ie(e))
            n = 16;
        else if (typeof e == "object")
            if (i & 65) {
                const s = e.default;
                s && (s._c && (s._d = !1),
                ha(t, s()),
                s._c && (s._d = !0));
                return
            } else {
                n = 32;
                const s = e._;
                !s && !$u(e) ? e._ctx = Gt : s === 3 && Gt && (Gt.slots._ === 1 ? e._ = 1 : (e._ = 2,
                t.patchFlag |= 1024))
            }
        else
            re(e) ? (e = {
                default: e,
                _ctx: Gt
            },
            n = 32) : (e = String(e),
            i & 64 ? (n = 16,
            e = [Fu(e)]) : n = 8);
        t.children = e,
        t.shapeFlag |= n
    }
    function x_(...t) {
        const e = {};
        for (let n = 0; n < t.length; n++) {
            const i = t[n];
            for (const s in i)
                if (s === "class")
                    e.class !== i.class && (e.class = ce([e.class, i.class]));
                else if (s === "style")
                    e.style = os([e.style, i.style]);
                else if (pr(s)) {
                    const r = e[s]
                      , o = i[s];
                    o && r !== o && !(ie(r) && r.includes(o)) && (e[s] = r ? [].concat(r, o) : o)
                } else
                    s !== "" && (e[s] = i[s])
        }
        return e
    }
    function Kt(t, e, n, i=null) {
        sn(t, e, 7, [n, i])
    }
    const I_ = vu();
    let C_ = 0;
    function R_(t, e, n) {
        const i = t.type
          , s = (e ? e.appContext : t.appContext) || I_
          , r = {
            uid: C_++,
            vnode: t,
            type: i,
            parent: e,
            appContext: s,
            root: null,
            next: null,
            subTree: null,
            effect: null,
            update: null,
            job: null,
            scope: new zc(!0),
            render: null,
            proxy: null,
            exposed: null,
            exposeProxy: null,
            withProxy: null,
            provides: e ? e.provides : Object.create(s.provides),
            ids: e ? e.ids : ["", 0, 0],
            accessCache: null,
            renderCache: [],
            components: null,
            directives: null,
            propsOptions: Eu(i, s),
            emitsOptions: Lu(i, s),
            emit: null,
            emitted: null,
            propsDefaults: Ae,
            inheritAttrs: i.inheritAttrs,
            ctx: Ae,
            data: Ae,
            props: Ae,
            attrs: Ae,
            slots: Ae,
            refs: Ae,
            setupState: Ae,
            setupContext: null,
            suspense: n,
            suspenseId: n ? n.pendingId : 0,
            asyncDep: null,
            asyncResolved: !1,
            isMounted: !1,
            isUnmounted: !1,
            isDeactivated: !1,
            bc: null,
            c: null,
            bm: null,
            m: null,
            bu: null,
            u: null,
            um: null,
            bum: null,
            da: null,
            a: null,
            rtg: null,
            rtc: null,
            ec: null,
            sp: null
        };
        return r.ctx = {
            _: r
        },
        r.root = e ? e.root : r,
        r.emit = y_.bind(null, r),
        t.ce && t.ce(r),
        r
    }
    let Qe = null;
    const fs = () => Qe || Gt;
    let tr, bo;
    {
        const t = mr()
          , e = (n, i) => {
            let s;
            return (s = t[n]) || (s = t[n] = []),
            s.push(i),
            r => {
                s.length > 1 ? s.forEach(o => o(r)) : s[0](r)
            }
        }
        ;
        tr = e("__VUE_INSTANCE_SETTERS__", n => Qe = n),
        bo = e("__VUE_SSR_SETTERS__", n => ds = n)
    }
    const ws = t => {
        const e = Qe;
        return tr(t),
        t.scope.on(),
        () => {
            t.scope.off(),
            tr(e)
        }
    }
      , al = () => {
        Qe && Qe.scope.off(),
        tr(null)
    }
    ;
    function Gu(t) {
        return t.vnode.shapeFlag & 4
    }
    let ds = !1;
    function L_(t, e=!1, n=!1) {
        e && bo(e);
        const {props: i, children: s} = t.vnode
          , r = Gu(t);
        r_(t, i, r, e),
        c_(t, s, n);
        const o = r ? N_(t, e) : void 0;
        return e && bo(!1),
        o
    }
    function N_(t, e) {
        const n = t.type;
        t.accessCache = Object.create(null),
        t.proxy = new Proxy(t.ctx,Xp);
        const {setup: i} = n;
        if (i) {
            Nn();
            const s = t.setupContext = i.length > 1 ? M_(t) : null
              , r = ws(t)
              , o = Ts(i, t, 0, [t.props, s])
              , a = Nc(o);
            if (Dn(),
            r(),
            (a || t.sp) && !Zi(t) && _u(t),
            a) {
                if (o.then(al, al),
                e)
                    return o.then(l => {
                        ll(t, l)
                    }
                    ).catch(l => {
                        gr(l, t, 0)
                    }
                    );
                t.asyncDep = o
            } else
                ll(t, o)
        } else
            Uu(t)
    }
    function ll(t, e, n) {
        re(e) ? t.type.__ssrInlineRender ? t.ssrRender = e : t.render = e : xe(e) && (t.setupState = au(e)),
        Uu(t)
    }
    function Uu(t, e, n) {
        const i = t.type;
        t.render || (t.render = i.render || en);
        {
            const s = ws(t);
            Nn();
            try {
                Jp(t)
            } finally {
                Dn(),
                s()
            }
        }
    }
    const D_ = {
        get(t, e) {
            return Je(t, "get", ""),
            t[e]
        }
    };
    function M_(t) {
        const e = n => {
            t.exposed = n || {}
        }
        ;
        return {
            attrs: new Proxy(t.attrs,D_),
            slots: t.slots,
            emit: t.emit,
            expose: e
        }
    }
    function ma(t) {
        return t.exposed ? t.exposeProxy || (t.exposeProxy = new Proxy(au(wp(t.exposed)),{
            get(e, n) {
                if (n in e)
                    return e[n];
                if (n in es)
                    return es[n](t)
            },
            has(e, n) {
                return n in e || n in es
            }
        })) : t.proxy
    }
    function F_(t) {
        return re(t) && "__vccOpts"in t
    }
    const Ft = (t, e) => xp(t, e, ds);
    function ju(t, e, n) {
        const i = arguments.length;
        return i === 2 ? xe(e) && !ie(e) ? er(e) ? ft(t, null, [e]) : ft(t, e) : ft(t, null, e) : (i > 3 ? n = Array.prototype.slice.call(arguments, 2) : i === 3 && er(n) && (n = [n]),
        ft(t, e, n))
    }
    const G_ = "3.5.13";
    /**
* @vue/runtime-dom v3.5.13
* (c) 2018-present Yuxi (Evan) You and Vue contributors
* @license MIT
**/
    let yo;
    const cl = typeof window != "undefined" && window.trustedTypes;
    if (cl)
        try {
            yo = cl.createPolicy("vue", {
                createHTML: t => t
            })
        } catch (t) {}
    const Bu = yo ? t => yo.createHTML(t) : t => t
      , U_ = "http://www.w3.org/2000/svg"
      , j_ = "http://www.w3.org/1998/Math/MathML"
      , fn = typeof document != "undefined" ? document : null
      , ul = fn && fn.createElement("template")
      , B_ = {
        insert: (t, e, n) => {
            e.insertBefore(t, n || null)
        }
        ,
        remove: t => {
            const e = t.parentNode;
            e && e.removeChild(t)
        }
        ,
        createElement: (t, e, n, i) => {
            const s = e === "svg" ? fn.createElementNS(U_, t) : e === "mathml" ? fn.createElementNS(j_, t) : n ? fn.createElement(t, {
                is: n
            }) : fn.createElement(t);
            return t === "select" && i && i.multiple != null && s.setAttribute("multiple", i.multiple),
            s
        }
        ,
        createText: t => fn.createTextNode(t),
        createComment: t => fn.createComment(t),
        setText: (t, e) => {
            t.nodeValue = e
        }
        ,
        setElementText: (t, e) => {
            t.textContent = e
        }
        ,
        parentNode: t => t.parentNode,
        nextSibling: t => t.nextSibling,
        querySelector: t => fn.querySelector(t),
        setScopeId(t, e) {
            t.setAttribute(e, "")
        },
        insertStaticContent(t, e, n, i, s, r) {
            const o = n ? n.previousSibling : e.lastChild;
            if (s && (s === r || s.nextSibling))
                for (; e.insertBefore(s.cloneNode(!0), n),
                !(s === r || !(s = s.nextSibling)); )
                    ;
            else {
                ul.innerHTML = Bu(i === "svg" ? "<svg>".concat(t, "</svg>") : i === "mathml" ? "<math>".concat(t, "</math>") : t);
                const a = ul.content;
                if (i === "svg" || i === "mathml") {
                    const l = a.firstChild;
                    for (; l.firstChild; )
                        a.appendChild(l.firstChild);
                    a.removeChild(l)
                }
                e.insertBefore(a, n)
            }
            return [o ? o.nextSibling : e.firstChild, n ? n.previousSibling : e.lastChild]
        }
    }
      , z_ = Symbol("_vtc");
    function V_(t, e, n) {
        const i = t[z_];
        i && (e = (e ? [e, ...i] : [...i]).join(" ")),
        e == null ? t.removeAttribute("class") : n ? t.setAttribute("class", e) : t.className = e
    }
    const fl = Symbol("_vod")
      , H_ = Symbol("_vsh")
      , K_ = Symbol("")
      , q_ = /(^|;)\s*display\s*:/;
    function W_(t, e, n) {
        const i = t.style
          , s = Be(n);
        let r = !1;
        if (n && !s) {
            if (e)
                if (Be(e))
                    for (const o of e.split(";")) {
                        const a = o.slice(0, o.indexOf(":")).trim();
                        n[a] == null && Us(i, a, "")
                    }
                else
                    for (const o in e)
                        n[o] == null && Us(i, o, "");
            for (const o in n)
                o === "display" && (r = !0),
                Us(i, o, n[o])
        } else if (s) {
            if (e !== n) {
                const o = i[K_];
                o && (n += ";" + o),
                i.cssText = n,
                r = q_.test(n)
            }
        } else
            e && t.removeAttribute("style");
        fl in t && (t[fl] = r ? i.display : "",
        t[H_] && (i.display = "none"))
    }
    const dl = /\s*!important$/;
    function Us(t, e, n) {
        if (ie(n))
            n.forEach(i => Us(t, e, i));
        else if (n == null && (n = ""),
        e.startsWith("--"))
            t.setProperty(e, n);
        else {
            const i = Y_(t, e);
            dl.test(n) ? t.setProperty(ui(i), n.replace(dl, ""), "important") : t[i] = n
        }
    }
    const pl = ["Webkit", "Moz", "ms"]
      , Hr = {};
    function Y_(t, e) {
        const n = Hr[e];
        if (n)
            return n;
        let i = xn(e);
        if (i !== "filter" && i in t)
            return Hr[e] = i;
        i = Fc(i);
        for (let s = 0; s < pl.length; s++) {
            const r = pl[s] + i;
            if (r in t)
                return Hr[e] = r
        }
        return e
    }
    const _l = "http://www.w3.org/1999/xlink";
    function hl(t, e, n, i, s, r=sp(e)) {
        i && e.startsWith("xlink:") ? n == null ? t.removeAttributeNS(_l, e.slice(6, e.length)) : t.setAttributeNS(_l, e, n) : n == null || r && !Uc(n) ? t.removeAttribute(e) : t.setAttribute(e, r ? "" : Ln(n) ? String(n) : n)
    }
    function ml(t, e, n, i, s) {
        if (e === "innerHTML" || e === "textContent") {
            n != null && (t[e] = e === "innerHTML" ? Bu(n) : n);
            return
        }
        const r = t.tagName;
        if (e === "value" && r !== "PROGRESS" && !r.includes("-")) {
            const a = r === "OPTION" ? t.getAttribute("value") || "" : t.value
              , l = n == null ? t.type === "checkbox" ? "on" : "" : String(n);
            (a !== l || !("_value"in t)) && (t.value = l),
            n == null && t.removeAttribute(e),
            t._value = n;
            return
        }
        let o = !1;
        if (n === "" || n == null) {
            const a = typeof t[e];
            a === "boolean" ? n = Uc(n) : n == null && a === "string" ? (n = "",
            o = !0) : a === "number" && (n = 0,
            o = !0)
        }
        try {
            t[e] = n
        } catch (a) {}
        o && t.removeAttribute(s || e)
    }
    function X_(t, e, n, i) {
        t.addEventListener(e, n, i)
    }
    function J_(t, e, n, i) {
        t.removeEventListener(e, n, i)
    }
    const gl = Symbol("_vei");
    function Q_(t, e, n, i, s=null) {
        const r = t[gl] || (t[gl] = {})
          , o = r[e];
        if (i && o)
            o.value = i;
        else {
            const [a,l] = Z_(e);
            if (i) {
                const c = r[e] = nh(i, s);
                X_(t, a, c, l)
            } else
                o && (J_(t, a, o, l),
                r[e] = void 0)
        }
    }
    const bl = /(?:Once|Passive|Capture)$/;
    function Z_(t) {
        let e;
        if (bl.test(t)) {
            e = {};
            let i;
            for (; i = t.match(bl); )
                t = t.slice(0, t.length - i[0].length),
                e[i[0].toLowerCase()] = !0
        }
        return [t[2] === ":" ? t.slice(3) : ui(t.slice(2)), e]
    }
    let Kr = 0;
    const eh = Promise.resolve()
      , th = () => Kr || (eh.then( () => Kr = 0),
    Kr = Date.now());
    function nh(t, e) {
        const n = i => {
            if (!i._vts)
                i._vts = Date.now();
            else if (i._vts <= n.attached)
                return;
            sn(ih(i, n.value), e, 5, [i])
        }
        ;
        return n.value = t,
        n.attached = th(),
        n
    }
    function ih(t, e) {
        if (ie(e)) {
            const n = t.stopImmediatePropagation;
            return t.stopImmediatePropagation = () => {
                n.call(t),
                t._stopped = !0
            }
            ,
            e.map(i => s => !s._stopped && i && i(s))
        } else
            return e
    }
    const yl = t => t.charCodeAt(0) === 111 && t.charCodeAt(1) === 110 && t.charCodeAt(2) > 96 && t.charCodeAt(2) < 123
      , sh = (t, e, n, i, s, r) => {
        const o = s === "svg";
        e === "class" ? V_(t, i, o) : e === "style" ? W_(t, n, i) : pr(e) ? Jo(e) || Q_(t, e, n, i, r) : (e[0] === "." ? (e = e.slice(1),
        !0) : e[0] === "^" ? (e = e.slice(1),
        !1) : rh(t, e, i, o)) ? (ml(t, e, i),
        !t.tagName.includes("-") && (e === "value" || e === "checked" || e === "selected") && hl(t, e, i, o, r, e !== "value")) : t._isVueCE && (/[A-Z]/.test(e) || !Be(i)) ? ml(t, xn(e), i, r, e) : (e === "true-value" ? t._trueValue = i : e === "false-value" && (t._falseValue = i),
        hl(t, e, i, o))
    }
    ;
    function rh(t, e, n, i) {
        if (i)
            return !!(e === "innerHTML" || e === "textContent" || e in t && yl(e) && re(n));
        if (e === "spellcheck" || e === "draggable" || e === "translate" || e === "form" || e === "list" && t.tagName === "INPUT" || e === "type" && t.tagName === "TEXTAREA")
            return !1;
        if (e === "width" || e === "height") {
            const s = t.tagName;
            if (s === "IMG" || s === "VIDEO" || s === "CANVAS" || s === "SOURCE")
                return !1
        }
        return yl(e) && Be(n) ? !1 : e in t
    }
    const oh = it({
        patchProp: sh
    }, B_);
    let vl;
    function ah() {
        return vl || (vl = f_(oh))
    }
    const lh = (...t) => {
        const e = ah().createApp(...t)
          , {mount: n} = e;
        return e.mount = i => {
            const s = uh(i);
            if (!s)
                return;
            const r = e._component;
            !re(r) && !r.render && !r.template && (r.template = s.innerHTML),
            s.nodeType === 1 && (s.textContent = "");
            const o = n(s, !1, ch(s));
            return s instanceof Element && (s.removeAttribute("v-cloak"),
            s.setAttribute("data-v-app", "")),
            o
        }
        ,
        e
    }
    ;
    function ch(t) {
        if (t instanceof SVGElement)
            return "svg";
        if (typeof MathMLElement == "function" && t instanceof MathMLElement)
            return "mathml"
    }
    function uh(t) {
        return Be(t) ? document.querySelector(t) : t
    }
    const kl = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAHgAAAB4CAYAAAA5ZDbSAAAAAXNSR0IArs4c6QAAAERlWElmTU0AKgAAAAgAAYdpAAQAAAABAAAAGgAAAAAAA6ABAAMAAAABAAEAAKACAAQAAAABAAAAeKADAAQAAAABAAAAeAAAAAAI4lXuAAALpElEQVR4Ae1dfWwcRxV/s3tnO+fYsePYifMhCrSENBVKi0ihKaSoBYoAgRAE/gCVCgn/kSaIUgqIAguoFQqlFUlplRCKVJAoNRTaVEpaQZM2opEqJW1MbD6CS5W0cRM7H65dn313u8Pv3fr8lfPHJvHNju/N6e72Zmd3Z36/eW8+3ps5IgmCgCAgCAgCgoAgIAgIAoKAICAICAKCgCAgCAgCgoAgIAgIAoKAICAICAKCgCAgCAgCgoAgIAgIAoKAICAICAKCgCAwIwTUjFLFNJHWWn2XHqpLDGWafCdRl0jmyCWnb4ick/fQxjNKKR3TrJcsW9YSvElvrawhf5VLFdcp0u8hCpZochwifVZR8C+faH8D+QdvV7enS4ZmDB9kJcFMbj1VXK8p9yUU4DrgugTvKpALglUW77OadBt+/xYV4C+eahmIIfYlyVKiJE+5xA+pI+carbOboYJvwK1r+fYglL8QdAKkz1OkFoPoJVlKD+HU4wQxD8+X1ydqvF3hm3r7IhB4i1LOjch5LRM7Sm5YljAmcJFujUvJr/+Etl1mVykvXW6tI7iWMu+FKH4S5FVPJHYiLAGod0m9P0PqUxPPlctvuwjWpHKkPuSQag4omAFHrJVVEp8f97RnZXM0g0JOmcQqgj3yXLSxV6p8b3nKco2c5IqAQl6RpubqkcgyOrCKYPDC+a2Pyg8kuGYe5eZFvW4upLeNYMY8kqplJY2AclbZWNYw9xfxWZaFvgi8rLtUCLaOsmgZFoKj4WVdaiHYOsqiZVgIjoaXdamFYOsoi5ZhITgaXtalFoKtoyxahoXgaHhZl1oIto6yaBkWgqPhZV3qSPO61pWuSIZhNkSlXlCbJV2dpISCg166knp6PeXliiS3Pgp2c3sCyKnwqeFZlxLrfJoZHzAtwuwfdFdS4uq3yK92KHifQ867YHZcBPMyvD40/LfovwGpl/ooaN+mNg/Zg8j0OS0LCQaRziAFNyeJ1juUWAsbcTMcAapQuyHNahD+W904aKsn9YSn793lqTt6vqa3J5dSUJ+mTFWCfB/S3ufRpj6yzBV3zkvwcB1nEjtxvBLfidBnKzQk4jei8zCwS+ZRSPLDkPjnEc+uuCuRtsYhN+NT8AY8SY7AHffQ3XTbMVuc+MqFYPDBbpXD1uFJNBtUN6d5DWk7AMwqn3QTjrlCBFD1fTh7DKr976gQj15JjQc2qA3gO94BlbY8wnTkMgrs3oMqsBxpPwqPrxUgsxLSzG5C0O7BQpxbg/hbQfz3DlP3BwrI5TtuaAcKv+P0XRZtcDTAC1VhvLRzLL8g0SlIxU0gvf+u3NYlYDWF7l7lD+iBTKB5VYU+jhrR6anNb0Z77uykjg/BwO5O+vX8FA3UZCnjBlQ5EMfhS0i/gnO9+pjjJt4NWlIgFRJOOUerc77Sx0D4AU8/uLud9ra3qlajaty8WgGxHv0C6tC5Bv6Pq6ESlyJTcHUN+uB+1RlQ7uUhyhy+V33rrQsZJs2OXIR3Ddvs0ScwmJB7dNb0KajxvWD9gYBOH8AYeyY+vqM3uoRHRiWYhyLLKVibpdwXXXLXAZvLIBnzARK0oJvDx2msTDiCc/u+rx98ppYGO85yUxmTMNE3e1ipK1TUxSjHZ7IUVGhqvAvZ/aepLBuTYO6YBFS/HmsP7gAY65GR/EqFUAWGcPAkRZhBdQYxLwDQP6MS3IZhy9UBGdV80/KFMiGN6kd57ktTegtroGkvmoUEBiV40eUo/GYA8RGQlsz3XycUEGqaVR53bBbi8xPovKzCOLWO4+MeuKKies5H2T5bS9VPIr8HTeTZyDBpO1QzKPo0JPRGJnes1BYDgc+DVKg+eieOGqZLX+weJuJCFe5cDlX9wfxQykAmjBDchTYKJN0MJVYThawoaQ1gOckjFdYt07UDtMLI0hkjBA9R+u1QuSsnQWRORfP8GcIVKRpqNFEwIwQnfb0IqnmBnRIZnSaQ3Ii57PIhOHAdVOvy2CCF5RdvDP1U5EVz0avS+VcYkeAKUidR7F7uH8/9kFfRlRiupEyU1QjBAbmdGPq8VA70DpPqYGbLyJDUCMHt1HDaJ/UH1O3XebqvHIJraGLGCLqtsKMmKf00Olk7IMXHmeRw5mduUo0y6hwljMzOGCGYaWS3GJf0zhwFP0cH5DlEvTlXiUb5fBggjMytGiM4JHnTiQylHkYF/xE6XNvw/TKkGt4TqPNzKGCYBIIJG7SVPhhp+McWc4v6KsyCtNfT2/8RUAYdL9WCGn8DKJ52CnPsfeJ8jL7GEObQjRgbjErwWFKw3WCPQ1c9AVV2D2r8s5Bofy5I8nAZ+gE0LJ2lD7EhmIvuqQ/n2mnxfpC8BWbfF0sPx2w8kZsbdQrOC2zyLHmIFcFceu5hn6CK/TD434/2GOPl2GUxIklsNtSdA3ROJLiA3A7VkgUou2H23Ym4XltJZvUMMyd8qlVbiu4s7za4QG7h21Mb+zFl/TvYVPegd21pzzo/GngD+T/oKTZnlz7EWv/B9RRO6O4v0YZ14F16dC76iWwvUwcDqmq/6Ftd4A1iTTCX6RzlXoS6/g0O+2zqVYfqWbNv9J41tKCHy2IixJ5gXu2XJdWKbfqfs41gEHoIfmT7TC5xiT3BXOvvVhuPQ9XtRHvcZUOHiysi3AUxgaN2ETW9ymUwFawgmMFJUXIfvgBY/DtcoabRhzRld3tqQ4bzbypYQ/B3VAscBOgRdFv+F2dVHUovwR/a2ZWgZZ2miC081xqCOcP1lD2kAv04SM7Fl+Q8xUcqSO8xLb2MmVUE838gaSf5e+T7aBwJDvOkWSU/HVC1cem1jmDOcCNleXE2lrBQXKX4hM7pvZ66dZDzazpYJcEM1mYMm0Dwn9Cjjl1bPCzB7bkEYWImHsE6gkPYqlmKn4KdNTYmRSYX+dHoHxyupDNGDAvFqpSVBLP6g5fiozApgujwVaxwpY/TaUytdsRpzy0rCWbiknSmDQLzEA5f4d+heuQjMyGUYP0a9uH6t5kcFH+qtQRDSgYdqsT+CMFWDAbYOaCfaWbHvVK/hsk9jQr31CB8votDbSbWRhPNOKQ8fd9CyPP1sLnyUtSrsBsAlpcG2B0HbJcksBkw6EXb+wJRxSM/ppa2OP1vcYlAmF2kPb0XzoP/WeqT/zZFfhPITs3uE0fvDkuXBoinsesAVPOpY3FqfzmXc4LgUbiJHtOPuR3UASNOaUIzNesu6vJBrBGDfmlKKU8RBAQBMwjMCRX9bf3TBRWUege2L8LGoXDfmjaM9/fXPF8yHIIcrh9/GrbdQCUmRubTjybErj8K7TFPvHS71HMUKtuombBQnpGCFSJs+/b0z5oCSn0Zvdh1KAz+EXxMvwJeAhqvYmUqGjmScOI1xVOPfRaPN5EHbILmnMRjnzxL/h/jsPf0aBUcKZxdB3Bo+wLWJn4DUoY/jVbjmcj/KkyBgI5Jw/jLxtaRSS/BifCqsDKEd89PV2L3Und1PWl4U9Lfprq+FOesnehgcDx9fx1gvgXvZQDYAbxg+Pw3ZIu5mOLN5y/0PZ4mSDFjuhrvz2PrJOMCZDXB+E9gJnhp3DZG4+3bUOGWzaMVxv+U2ngNG1//I/8aAJSvQAk3FxRx5DvMwgWocLAF61e7ifd7MxusJriZnLOvU64VUoyN1Wg5oMQGr6xqL2QHH9xhkjD2DOvySZJxNB7Oy0SDw/j+61qqMm7053bJ6uDpXy33Kf05kHotpLkudKsNBWcqJpimcM54ZpUBhE243XjhDE/yklfqQj6ed6niGV4Saxpc6wnmqWCPdjTAgwdEB+dvF4jdud0E78s1XUBCBMwpj6SNsiTfJQd7NATaze9zXXPih/SV3jgYHewneAxvOj/vPxoxu4Xju4/UhTEPnSjpo6fkSBAQBAQBQUAQEAQEAUFAEBAEBAFBQBAQBAQBQUAQEAQEAUFAEBAEBAFBQBAQBAQBQUAQEAQEAUFAEBAEBAFBwG4E/g9UW5PxuVVsMQAAAABJRU5ErkJggg=="
      , Al = "/assets/pgsoft-BKaWenKb.png"
      , fh = "/assets/logo-askmebet-DcEE9zhG.png"
      , dh = "/assets/gambling%20commission-DxiLVeV5.png"
      , ph = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIoAAAAwCAYAAADO1uJ3AAAAAXNSR0IArs4c6QAAAERlWElmTU0AKgAAAAgAAYdpAAQAAAABAAAAGgAAAAAAA6ABAAMAAAABAAEAAKACAAQAAAABAAAAiqADAAQAAAABAAAAMAAAAABqZdM6AAAKz0lEQVR4Ae2be5DWVRnHQUpBRCxDwzSXpGkyocmCTKNIGRVQM5UErFy1MLvoH16mprJsJmtiSpqaITAj8gKIRTdNSxq1C5WXsmwqLfBCFmoIyoYLLNvnu+5Znn32nN973nffd9llf8/Ml985z+08v3Oec/mddxkypKSyB8oeKHug7IGyB/q2B9rb2+8Eb+vbVqtrjfjOBUuqsyq169oDDIDoEfDmujqukzPimg42gTvq5LJ0U0sPMACB1lF4Sy0+GmVDPDPAs0B0a6PaKf1m9MCLY9D172OUJmeYNVyFOGYCrSSBykRpeK8XNBBGwTwfp3xMgUnDRbR/BggrSQitTJSG93xBA2EU3HM99d1ywKXdM8FmF4+qZaIUjGPDRZEBCawnKRzb8ABMA7R3FnguBOCeZaKYvurzohsMX1WyHN8XQdHOe0EqSRRXmSh9MRCJNl6S4Af2WArLGaS5Q4cOvTMw6/3E/2x8LgajGuB7L3yOBEMjvlt5r9YIP8kiVvnbL6GwFX/bE7IebHztC3NEJ/bm2QIUz2b8tPPsFeFf76xY/bvvxP+WapxXShT5GgOW0eg5OP9ZNc5zdPE7F71FINX5OW6KdMYjXA72B7bzNeAP0P7ZvNdOyrn0IRSvAN5GgyH+90GSaE/JMROcCiaCQ4F4sm8DG8FD6KmvlxHbBp610rswVN96asP/efhe4wXJOga59DSK05OOahDg7/1gS2YANW09+J5Y0EYrskm5oaM7HKwBKTq/yBdGs8EfU8YRvq4rPgaGFflNybBbGvEZWF9N2UX5wSrzqWQ5MeqoSiZ+5oDcJFF4tSbKBGztfYx8Wbo6N3SMJoGt1tiVm2O+0BkBFjjdaqrXozw65jvFQ/8Q8ERBI2uRHZCy93wtv9XQK1C+iQZ6tbJg/z78fAvo7LC7aRbx7J8ZxCz0hmfqdqjh+6UUFoBLCuy0JWrbSZH6ayG+qml7Gjba1lI0DsHJKaHnV5sosj8Q3EjQp3hnOXXsPoCeDq7al/sDHUEQJ1QKhLg1o3WuqJYuwmBewuh2+BcDDdhUoIRYCp4DnubA0PmoIhGrzjv6QKhEuo6QbmVCsVbSzenplVvYpYH+uaBo6UacpEZtPWpw+a4o4yV09JPCTikXULO1Ru8I8FRE/3l4F4DoIME/Dmhr8LQOhlb1QkLnSOAvLTVe24CljVReXeisU1jLihL8an/TYemMwCh6oteM/JugmuWzyGU9ZScT37gKDjWjowNbYPdBZGOcXFvMpXxxXAfsV1iXGvxfU9Hq8b8u5ouFJh45Z0StfH47/Tq83wFLL6OSNdl7kyhqUMF8h07W3p0k5Och7K9Jori1rZymQoyI/zD4fnuKDnKwx0bnr9ggaGW8NuilniTLamS3ReSFiUK7+2Djx0N3Jtcn/Ol3tYrXJBUVIoF6li7JltDYXrzcCi+Efz68hWBvL9uN9X/TditoMjHoK2wh77DN8EJRg/PKUOG5CTwOJhqeL74BRpNj7qCuNgqTzNjMp7wehIOuJvYjRh4rvhXmUU7wAPV/gB+DK4Fd1Y+hrlgfBEmqR6LIuWbPtzuTZVlojfoFlJUkOvn3J1pLMOqYj5ig9IdbU4BmchfxDsOo6FLQ0u+pyEdRooxHbgdE9hr0e1XIIRJK7QjV0Jkoa1WxdIuSE/o7TG0/7zTCsAL1SaKo3X2BkuVgnneBk8DnQX9LEkLqOGtoKZ4HwmTRczbolijUNTsnA0s3UplgGZHyYRHeegbsv55Pn+ns8w4w1ssidZ1bfoqfHj8V4EdnDr+FPgtPt7xDZIOOVhWbKBKdBv+LyFtUiVHopJisFp5m0DVADWqV6a+k5L0f3Ae09AbSX9SNocOeDgye6vj9TH0DZZ0djja8WDH2/htjivCUKJ8DU0ElegaF1wJtf56Oh3G4Y2oFedjwfkD5s0BHhkCaDNqyfhEY/qk9rxEU66RGtFOrT52nNCOXOweHUO+akSTNCOr+YHgbthqsSpOs3flWVdtYinRmyqEXUIr5lu3ZQEln6WbitfrrEP7KKlCWjVbTJNU7Uf5MS39Ittb/BKsIyW8F+nOH0C/HIn+9CbuN8g2mXlTcHBEW3YH0aosm5ibaO8G1+ST1Wy2PpNlJXRPEJo9UdEUwRoUYVZoVMZsUT41fCDRTNQA6o/R3eoIAbwfnmECnUFZy/AWcBWwf6cD3G5BD/4oo6feXgxispyKyu+E9DzSQgTSYU8HLA6PgOT2ip/OMvuZC4stcPg8CSnr7bjpTTQPLQE/CST1IV/pd2w3lL9XDqfPRbWb0fJM4Bx+xHwXvC9rINZP8jetV8EYB/f2wpcuM3TesoLPcbORHw2txOjuozww6lZ7ojgR/cz70Q99oa0t9GLjb6dVSXYWR37o6mrKZZtuupnwDyvOYJS3GqB5+jbuGFu/Bu7ZMSzOozAGvMkwdRLVS5pJWpH86ZZ1RPpwaDKer6nHgNRG+Z70RxmTPrKE+FZtoe70d0O/i+EKXJDXEt/tMiF3L8y0ugjdR/wyw/fNzdP3AO7NdVXR1OF2xi9NV0hZxUVctUSCZDkCkGHLOLu9GT1+cvSW1eWrMid2jYvIi3hKEH6VDthYpDRCZzleXg/DJqJl/qIld+7omRbW0FAMlhV2Z5Hs+iaAEWBzrP2SvQ3YNeDsoJHR1fzUronQHvA3AJrtV01lIE2KCZVLW/4LQ7XG3r7BaE+U6HH489pKu0YFS1fX2anB6ImBtI79MyJJs+kf/7eVKFNRfljS4C8Bc5BrQvwL9aYE+z/WlpVl9IMihKSgpsSytp9JM+/+xTF+mbbXzQ2DPJZOoK3m6znGUu516Vc+ha1G6mCD0Pb9HEO9Cn7XfxMukEmUlOvoiqYW08h4JLo0Y61yRc7bQimYH07rSr/d+1VhdKUk6HdzD81EwrrOuxz5AK1S3RPENSLGIFiHUSrLHJIl5WX0ma2Z70ky/2TNz60pCdD8JrgYqV0MtKH8BRFcGknssMp1PPGXFS2ybMVzljamfgu9Rll9Nomj5vATnrdbBAChrJvoDYY8tl/fSirEi8j6adQ9H+DpreIr2J763g0+hrJvTe71Rov6nTv2reNqrf/su70F2sLNfS/23jldU/UlEqBXwRMvv0WFWaMqLKWsl2WZ4A6Wor5r7gWaIZrQGU2eOGK2EOQOEwVCSLeK97SVYsHuUwoMgyKT7DEgSflYyUzUwamMaOAo0gdGgDWjleAhoddPVe8d2h81d1HcAxS8d3ceovfHAxqB3W4WdTSxYhbQG6Y/A4cC+y0nUvwc6qGNzDpXEcxF8Jcn2hLwHm5f4MszLewh6x9BvLNmXVaEpYlHnad8NpA5uw1d0ZUR/OHLpdBB6W0PZPtFTMgl2O9mGvgY8izp9jEQ5+HkB+y3e2LXVjk7H1g9/BLqKNcSgsv5TW3YMasv5F0s0zMZSaUX5GgaXYaBsHpBE7Jol0cGOvRD6Wecv9DRxsidPoi3Zb4rJLC/VFvzs97L+fDnl3+oVJYo+3/S3nWE5snZleZD1QPTwRR/osqdMkkGWDEWvG0uUr2Cg7aZcSYp6bpDJfKLM5/2vKJNkkGVBxuvaRNGXyifKJMnotUGoosOsvsuXgE/XMUm6Pi/r2Kc2qevotnSV0wNKFP0I9Vgdk0Tt6kc23WbW67Na9wypSzJEJZU9UPZA2QNlD5Q9UPbAHtYD/wenQ1TMkgRyUgAAAABJRU5ErkJggg=="
      , _h = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEIAAAAwCAYAAAClvqwiAAAAAXNSR0IArs4c6QAAAERlWElmTU0AKgAAAAgAAYdpAAQAAAABAAAAGgAAAAAAA6ABAAMAAAABAAEAAKACAAQAAAABAAAAQqADAAQAAAABAAAAMAAAAABrvzK4AAAI70lEQVRoBeWaZ4xVRRSAWcBFRQVRsVNEUOyKsRfEBCSKYostRkWNGo0hsfwyaIw9xhKNiSVqSDARNRbs3Rh7CSoo2GBBQEVwUZSyFL/v5c3mvnlz990tBJCTfDv93JkzM2fm3rd1ndYTWb16dWe6uiVsBb2gdzm+LeH2ZTYiXAyLoAG+hNkwt66uzvxcqcstWQ8LMFYPut0fdoN+sCNsAhqxEd6CdzHKv4QV8r8yRMXIygmMsznRraEf7Ac7wGfwCQZpICzJ/94QYaAhxDBdiR8MO8MCcIU0bXCGYODNglH6k9DvTG3O3JAiGGBTcLt0IuwC9TqRDVGWMeiBGGAQ22IlLF9jhuAha3zb+Yy2PMfBY4gpcDrtPYo7dUhnUeYy2ws8tvrCdqBTWgLz4Ef4Cb6GLWAfWA1KF2igc9+WUjl/eIb3hT1gELi3TdtWPX/CXJgu6JpGWFPQeR6VdqD+rTUrt1QBRbvC7fAVNEFL8g+FH8PniUq35z2HunvD/TAVVkAt+YUKz8KxeTpDPnXs/zugw2y90LAzXAhzoSPkirgXKO0FN8GiNj5gGe1uAS9USaHMZ0wG7xetExpphBtgJXSUjM72AqX94IMOUn4nepIugPye8B2c2BZnOZZOXw9taZsdb4jruBaGRDlcTqgv6QhxtZ2Uo6gb+fqzOh1aYcFyQ6g8roUGv1Km0/sOvM9vDLvAYPCBm0IsOrrGbCbOyy13LnmTYKdMmXXngM7XNs50DxgIO0PKeA72MvS9it6lxLNiu54wK5vZYhxFHlXjISXu49ugX0oJ+V5gRsF0iOUHMvrktBtNmU52FtwMQ6BqmZPndj0cJkFKGsl0QiqEPLfNHPD9o5hQ2QvIXxDLYjJG19JCna7wftyY9KewWV57ykbCgLzybL564D1IyfCobm8qNcDT0Lk1+3wEinyTi+VhltxzcWYibVvP/1gaab84zgxpyl4B7yA1paxnIhXDHSXbxq2TlXNIuBIfp92q1viI/bNayvFVhBMS+aks/UPpfh8V/h6lk0lmzb66hNXhdwZD97d98EPMXzAbvGStgI0gK/qKkqCrL5Fr4U14xcxChnDpUNevQLH8QkZRR6PTSz1PHbnCs213FgyDPSGeWbIqRMNU+RHySqsEffXE7wId6zhWw0rCZMfMj8U9nLp9OYjYE8dtQ9oBOVuxJA1Bh33mRTAWnMGikrfdG8sKLic8BW7GCB8FpV1DpEao9fxWGMscMooaIjWTzpLLuUIwgqvvQRhVUdC+xAz0noqK2+B5uCWrLs962TrGNUTpLS0q8KOol58ikjKEy7JiRdDZ7uQ9BB1phCb0HQaPwhS4gn57z2mWoitCj5/aGr7xFZWUIVbQONZxJXkn5Ch1BbkKJ4O+yQuWOrxUOVk604Mh7qvtfLHzsnc2RqgwPnnFhFk6AVJyfhENNPTC83ZCwUzyHEBJiO8G8xP1zPLd4wzoAVUTSJ4XPsuyd5VVpMM7kS9X3nCTUqUwWSvtqZ2J+Tn142xXlEddLM6MyzbI2URSR6x3g0uYyeDwQv3mkDLGWToRwhZ2FYjb/z04nzozCZNS1EeklvUSNP6R1Fqd6VLNM8RyqzMIz/3jjEfyM2n3dK4RMvU1eDCkR6jjGw+ntWQEygu/QaYM8Q/ti66IPEPMpoM6TKUveFGKZQJ1ij7Hl7zgH7xgXQUX077mhBXdGt4BYtHrLogzc9KuhmZfkKmTdZQaISzrTJXSjzHZdDLOitJRjgVX1mdwDQZwSxSSooZIDcIbXGlZF3iSA0xdpn7NtPUZ8dck/YcnQ65gANvoW8ZBb7gR7sUICwndcgMIupKebjpPivqIsHyzeuxAat9n64R4asnrbLMryr7o3LLiRLnck8Igh1HwLDwCM2A4A75eI1BWDxeQ9yL0hxalqCGyMxcU+qFleEjUCFOG+Js2WUO4p5dFenR4Fc9gcL7OHwkvUPYS6L/OhJEY4H1CV8HhBJPAC5RbZia0X1B8I6TEe8CZ4JtlhZBnhweVyz3DY/FbQPNMEfd7h1+mYvGjis/oBkPhCfBuMA2uhuZVSdx7yN3gN5IgfnhJbe2K/hZKoOhQWBo0R6Gd8svTy/AIjIc3wI+iv0GeTKHA464kxL10vZZTeTn5Gt0B2m4MbJVpO4C0X8hSz/s61Gt3yAPcbxOgI+XDuGMoHwFLyg/xVhjEvIkwGjwVSkL8AHgAUgYguyT6iJri8q1nb7Xo/S2n3nVo2xP2ram1WIV52Wro1/nq6afBfqCD1tPrBybAFPqhcbrDUaTHwEgI9waiSfH2WlP0yv7IoUPxpzIvSUmhbAb1TqbwPjg+Wal1mfoIT4SjYQQcBBpZY9wDr8MHPFcn2om6/eF0oifBoaAjLSJVr/mpRl7QPTn0vHZIQ8yEb+hAcpVQvxvlp8EFsDtsD/Hp4x3D2+Csctn+5dDjMQzADnYB6/4Jn8Iz4H+yhDuAE6XRL4EjwY81eeIpZN/qowpj0PdYlFeVDJ3S4sY9dobAQNAoX4HLswG8SXr2N6HYAdlGQ/SFXmXUYT1DZ9t6x4CGU2zvO8pU+AIcvPGp6Kz6wIP+sGJ8tXa7+KxtwAHbv3nwPahL+sAuUOofof14Et0/E7YoVqwSOrAtmfqDQ8qhA3UQy8AZ9EHSBGHQHlHuc2fNmXRFyWJogG9Bw06GhcGYxAsJfVKns23oM+2HfVraWl20qZKkIeJadKIHedtBT3Cgm4N3B2dGHRpInG1nyn29CObTybDHreuM7g1bgy9TGmn9EwwyFC4tz07NAVBvX3gOvFAtAI9B7x1Pgct+/RM6Phj09Ipfi/xvE1dGlZDfBbwG+z8NKTm3qtFazii6Nbakn8+DnjsrOrq3wdCtoD6X/WEwCrpDLN4TjmBbLIgL1uk00+kR+2hqWtuQ54XojHVxwPH5n+qjXvpH8Kxvr9yBgontVbJW2zOTB4KO719orfgr+rXQ/J6wVgfT3oczEN8QffF5ElKvzGRXiG+LGm9Ye5+9ptsXcpapTjC4PuQfAA5yHxgM3jF+By9Nb5XxHcbLzzot/wHpoFVmcDuh2wAAAABJRU5ErkJggg=="
      , hh = "/assets/bmm-l4cnuRBa.png"
      , mh = "/assets/glowing-JnIZjW1R.png"
      , gh = "/assets/scale-fVkRgtFq.png"
      , bh = "/assets/invalid_session_token_error-DlWkoO_3.png"
      , yh = "/assets/record_not_found_error-BeaFRuuJ.png"
      , vh = "/assets/internal_server_error-BMDpPUK-.png";
    function zu(t, e) {
        return function() {
            return t.apply(e, arguments)
        }
    }
    const {toString: kh} = Object.prototype
      , {getPrototypeOf: ga} = Object
      , vr = (t => e => {
        const n = kh.call(e);
        return t[n] || (t[n] = n.slice(8, -1).toLowerCase())
    }
    )(Object.create(null))
      , Vt = t => (t = t.toLowerCase(),
    e => vr(e) === t)
      , kr = t => e => typeof e === t
      , {isArray: Fi} = Array
      , ps = kr("undefined");
    function Ah(t) {
        return t !== null && !ps(t) && t.constructor !== null && !ps(t.constructor) && Pt(t.constructor.isBuffer) && t.constructor.isBuffer(t)
    }
    const Vu = Vt("ArrayBuffer");
    function $h(t) {
        let e;
        return typeof ArrayBuffer != "undefined" && ArrayBuffer.isView ? e = ArrayBuffer.isView(t) : e = t && t.buffer && Vu(t.buffer),
        e
    }
    const Th = kr("string")
      , Pt = kr("function")
      , Hu = kr("number")
      , Ar = t => t !== null && typeof t == "object"
      , Eh = t => t === !0 || t === !1
      , js = t => {
        if (vr(t) !== "object")
            return !1;
        const e = ga(t);
        return (e === null || e === Object.prototype || Object.getPrototypeOf(e) === null) && !(Symbol.toStringTag in t) && !(Symbol.iterator in t)
    }
      , wh = Vt("Date")
      , Sh = Vt("File")
      , Oh = Vt("Blob")
      , Ph = Vt("FileList")
      , xh = t => Ar(t) && Pt(t.pipe)
      , Ih = t => {
        let e;
        return t && (typeof FormData == "function" && t instanceof FormData || Pt(t.append) && ((e = vr(t)) === "formdata" || e === "object" && Pt(t.toString) && t.toString() === "[object FormData]"))
    }
      , Ch = Vt("URLSearchParams")
      , [Rh,Lh,Nh,Dh] = ["ReadableStream", "Request", "Response", "Headers"].map(Vt)
      , Mh = t => t.trim ? t.trim() : t.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
    function Ss(t, e, {allOwnKeys: n=!1}={}) {
        if (t === null || typeof t == "undefined")
            return;
        let i, s;
        if (typeof t != "object" && (t = [t]),
        Fi(t))
            for (i = 0,
            s = t.length; i < s; i++)
                e.call(null, t[i], i, t);
        else {
            const r = n ? Object.getOwnPropertyNames(t) : Object.keys(t)
              , o = r.length;
            let a;
            for (i = 0; i < o; i++)
                a = r[i],
                e.call(null, t[a], a, t)
        }
    }
    function Ku(t, e) {
        e = e.toLowerCase();
        const n = Object.keys(t);
        let i = n.length, s;
        for (; i-- > 0; )
            if (s = n[i],
            e === s.toLowerCase())
                return s;
        return null
    }
    const Qn = typeof globalThis != "undefined" ? globalThis : typeof self != "undefined" ? self : typeof window != "undefined" ? window : global
      , qu = t => !ps(t) && t !== Qn;
    function vo() {
        const {caseless: t} = qu(this) && this || {}
          , e = {}
          , n = (i, s) => {
            const r = t && Ku(e, s) || s;
            js(e[r]) && js(i) ? e[r] = vo(e[r], i) : js(i) ? e[r] = vo({}, i) : Fi(i) ? e[r] = i.slice() : e[r] = i
        }
        ;
        for (let i = 0, s = arguments.length; i < s; i++)
            arguments[i] && Ss(arguments[i], n);
        return e
    }
    const Fh = (t, e, n, {allOwnKeys: i}={}) => (Ss(e, (s, r) => {
        n && Pt(s) ? t[r] = zu(s, n) : t[r] = s
    }
    , {
        allOwnKeys: i
    }),
    t)
      , Gh = t => (t.charCodeAt(0) === 65279 && (t = t.slice(1)),
    t)
      , Uh = (t, e, n, i) => {
        t.prototype = Object.create(e.prototype, i),
        t.prototype.constructor = t,
        Object.defineProperty(t, "super", {
            value: e.prototype
        }),
        n && Object.assign(t.prototype, n)
    }
      , jh = (t, e, n, i) => {
        let s, r, o;
        const a = {};
        if (e = e || {},
        t == null)
            return e;
        do {
            for (s = Object.getOwnPropertyNames(t),
            r = s.length; r-- > 0; )
                o = s[r],
                (!i || i(o, t, e)) && !a[o] && (e[o] = t[o],
                a[o] = !0);
            t = n !== !1 && ga(t)
        } while (t && (!n || n(t, e)) && t !== Object.prototype);
        return e
    }
      , Bh = (t, e, n) => {
        t = String(t),
        (n === void 0 || n > t.length) && (n = t.length),
        n -= e.length;
        const i = t.indexOf(e, n);
        return i !== -1 && i === n
    }
      , zh = t => {
        if (!t)
            return null;
        if (Fi(t))
            return t;
        let e = t.length;
        if (!Hu(e))
            return null;
        const n = new Array(e);
        for (; e-- > 0; )
            n[e] = t[e];
        return n
    }
      , Vh = (t => e => t && e instanceof t)(typeof Uint8Array != "undefined" && ga(Uint8Array))
      , Hh = (t, e) => {
        const i = (t && t[Symbol.iterator]).call(t);
        let s;
        for (; (s = i.next()) && !s.done; ) {
            const r = s.value;
            e.call(t, r[0], r[1])
        }
    }
      , Kh = (t, e) => {
        let n;
        const i = [];
        for (; (n = t.exec(e)) !== null; )
            i.push(n);
        return i
    }
      , qh = Vt("HTMLFormElement")
      , Wh = t => t.toLowerCase().replace(/[-_\s]([a-z\d])(\w*)/g, function(n, i, s) {
        return i.toUpperCase() + s
    })
      , $l = ( ({hasOwnProperty: t}) => (e, n) => t.call(e, n))(Object.prototype)
      , Yh = Vt("RegExp")
      , Wu = (t, e) => {
        const n = Object.getOwnPropertyDescriptors(t)
          , i = {};
        Ss(n, (s, r) => {
            let o;
            (o = e(s, r, t)) !== !1 && (i[r] = o || s)
        }
        ),
        Object.defineProperties(t, i)
    }
      , Xh = t => {
        Wu(t, (e, n) => {
            if (Pt(t) && ["arguments", "caller", "callee"].indexOf(n) !== -1)
                return !1;
            const i = t[n];
            if (Pt(i)) {
                if (e.enumerable = !1,
                "writable"in e) {
                    e.writable = !1;
                    return
                }
                e.set || (e.set = () => {
                    throw Error("Can not rewrite read-only method '" + n + "'")
                }
                )
            }
        }
        )
    }
      , Jh = (t, e) => {
        const n = {}
          , i = s => {
            s.forEach(r => {
                n[r] = !0
            }
            )
        }
        ;
        return Fi(t) ? i(t) : i(String(t).split(e)),
        n
    }
      , Qh = () => {}
      , Zh = (t, e) => t != null && Number.isFinite(t = +t) ? t : e
      , qr = "abcdefghijklmnopqrstuvwxyz"
      , Tl = "0123456789"
      , Yu = {
        DIGIT: Tl,
        ALPHA: qr,
        ALPHA_DIGIT: qr + qr.toUpperCase() + Tl
    }
      , em = (t=16, e=Yu.ALPHA_DIGIT) => {
        let n = "";
        const {length: i} = e;
        for (; t--; )
            n += e[Math.random() * i | 0];
        return n
    }
    ;
    function tm(t) {
        return !!(t && Pt(t.append) && t[Symbol.toStringTag] === "FormData" && t[Symbol.iterator])
    }
    const nm = t => {
        const e = new Array(10)
          , n = (i, s) => {
            if (Ar(i)) {
                if (e.indexOf(i) >= 0)
                    return;
                if (!("toJSON"in i)) {
                    e[s] = i;
                    const r = Fi(i) ? [] : {};
                    return Ss(i, (o, a) => {
                        const l = n(o, s + 1);
                        !ps(l) && (r[a] = l)
                    }
                    ),
                    e[s] = void 0,
                    r
                }
            }
            return i
        }
        ;
        return n(t, 0)
    }
      , im = Vt("AsyncFunction")
      , sm = t => t && (Ar(t) || Pt(t)) && Pt(t.then) && Pt(t.catch)
      , Xu = ( (t, e) => t ? setImmediate : e ? ( (n, i) => (Qn.addEventListener("message", ({source: s, data: r}) => {
        s === Qn && r === n && i.length && i.shift()()
    }
    , !1),
    s => {
        i.push(s),
        Qn.postMessage(n, "*")
    }
    ))("axios@".concat(Math.random()), []) : n => setTimeout(n))(typeof setImmediate == "function", Pt(Qn.postMessage))
      , rm = typeof queueMicrotask != "undefined" ? queueMicrotask.bind(Qn) : typeof process != "undefined" && process.nextTick || Xu
      , R = {
        isArray: Fi,
        isArrayBuffer: Vu,
        isBuffer: Ah,
        isFormData: Ih,
        isArrayBufferView: $h,
        isString: Th,
        isNumber: Hu,
        isBoolean: Eh,
        isObject: Ar,
        isPlainObject: js,
        isReadableStream: Rh,
        isRequest: Lh,
        isResponse: Nh,
        isHeaders: Dh,
        isUndefined: ps,
        isDate: wh,
        isFile: Sh,
        isBlob: Oh,
        isRegExp: Yh,
        isFunction: Pt,
        isStream: xh,
        isURLSearchParams: Ch,
        isTypedArray: Vh,
        isFileList: Ph,
        forEach: Ss,
        merge: vo,
        extend: Fh,
        trim: Mh,
        stripBOM: Gh,
        inherits: Uh,
        toFlatObject: jh,
        kindOf: vr,
        kindOfTest: Vt,
        endsWith: Bh,
        toArray: zh,
        forEachEntry: Hh,
        matchAll: Kh,
        isHTMLForm: qh,
        hasOwnProperty: $l,
        hasOwnProp: $l,
        reduceDescriptors: Wu,
        freezeMethods: Xh,
        toObjectSet: Jh,
        toCamelCase: Wh,
        noop: Qh,
        toFiniteNumber: Zh,
        findKey: Ku,
        global: Qn,
        isContextDefined: qu,
        ALPHABET: Yu,
        generateString: em,
        isSpecCompliantForm: tm,
        toJSONObject: nm,
        isAsyncFn: im,
        isThenable: sm,
        setImmediate: Xu,
        asap: rm
    };
    function se(t, e, n, i, s) {
        Error.call(this),
        Error.captureStackTrace ? Error.captureStackTrace(this, this.constructor) : this.stack = new Error().stack,
        this.message = t,
        this.name = "AxiosError",
        e && (this.code = e),
        n && (this.config = n),
        i && (this.request = i),
        s && (this.response = s,
        this.status = s.status ? s.status : null)
    }
    R.inherits(se, Error, {
        toJSON: function() {
            return {
                message: this.message,
                name: this.name,
                description: this.description,
                number: this.number,
                fileName: this.fileName,
                lineNumber: this.lineNumber,
                columnNumber: this.columnNumber,
                stack: this.stack,
                config: R.toJSONObject(this.config),
                code: this.code,
                status: this.status
            }
        }
    });
    const Ju = se.prototype
      , Qu = {};
    ["ERR_BAD_OPTION_VALUE", "ERR_BAD_OPTION", "ECONNABORTED", "ETIMEDOUT", "ERR_NETWORK", "ERR_FR_TOO_MANY_REDIRECTS", "ERR_DEPRECATED", "ERR_BAD_RESPONSE", "ERR_BAD_REQUEST", "ERR_CANCELED", "ERR_NOT_SUPPORT", "ERR_INVALID_URL"].forEach(t => {
        Qu[t] = {
            value: t
        }
    }
    );
    Object.defineProperties(se, Qu);
    Object.defineProperty(Ju, "isAxiosError", {
        value: !0
    });
    se.from = (t, e, n, i, s, r) => {
        const o = Object.create(Ju);
        return R.toFlatObject(t, o, function(l) {
            return l !== Error.prototype
        }, a => a !== "isAxiosError"),
        se.call(o, t.message, e, n, i, s),
        o.cause = t,
        o.name = t.name,
        r && Object.assign(o, r),
        o
    }
    ;
    const om = null;
    function ko(t) {
        return R.isPlainObject(t) || R.isArray(t)
    }
    function Zu(t) {
        return R.endsWith(t, "[]") ? t.slice(0, -2) : t
    }
    function El(t, e, n) {
        return t ? t.concat(e).map(function(s, r) {
            return s = Zu(s),
            !n && r ? "[" + s + "]" : s
        }).join(n ? "." : "") : e
    }
    function am(t) {
        return R.isArray(t) && !t.some(ko)
    }
    const lm = R.toFlatObject(R, {}, null, function(e) {
        return /^is[A-Z]/.test(e)
    });
    function $r(t, e, n) {
        if (!R.isObject(t))
            throw new TypeError("target must be an object");
        e = e || new FormData,
        n = R.toFlatObject(n, {
            metaTokens: !0,
            dots: !1,
            indexes: !1
        }, !1, function(p, b) {
            return !R.isUndefined(b[p])
        });
        const i = n.metaTokens
          , s = n.visitor || u
          , r = n.dots
          , o = n.indexes
          , l = (n.Blob || typeof Blob != "undefined" && Blob) && R.isSpecCompliantForm(e);
        if (!R.isFunction(s))
            throw new TypeError("visitor must be a function");
        function c(m) {
            if (m === null)
                return "";
            if (R.isDate(m))
                return m.toISOString();
            if (!l && R.isBlob(m))
                throw new se("Blob is not supported. Use a Buffer instead.");
            return R.isArrayBuffer(m) || R.isTypedArray(m) ? l && typeof Blob == "function" ? new Blob([m]) : Buffer.from(m) : m
        }
        function u(m, p, b) {
            let E = m;
            if (m && !b && typeof m == "object") {
                if (R.endsWith(p, "{}"))
                    p = i ? p : p.slice(0, -2),
                    m = JSON.stringify(m);
                else if (R.isArray(m) && am(m) || (R.isFileList(m) || R.endsWith(p, "[]")) && (E = R.toArray(m)))
                    return p = Zu(p),
                    E.forEach(function(v, y) {
                        !(R.isUndefined(v) || v === null) && e.append(o === !0 ? El([p], y, r) : o === null ? p : p + "[]", c(v))
                    }),
                    !1
            }
            return ko(m) ? !0 : (e.append(El(b, p, r), c(m)),
            !1)
        }
        const f = []
          , d = Object.assign(lm, {
            defaultVisitor: u,
            convertValue: c,
            isVisitable: ko
        });
        function _(m, p) {
            if (!R.isUndefined(m)) {
                if (f.indexOf(m) !== -1)
                    throw Error("Circular reference detected in " + p.join("."));
                f.push(m),
                R.forEach(m, function(E, S) {
                    (!(R.isUndefined(E) || E === null) && s.call(e, E, R.isString(S) ? S.trim() : S, p, d)) === !0 && _(E, p ? p.concat(S) : [S])
                }),
                f.pop()
            }
        }
        if (!R.isObject(t))
            throw new TypeError("data must be an object");
        return _(t),
        e
    }
    function wl(t) {
        const e = {
            "!": "%21",
            "'": "%27",
            "(": "%28",
            ")": "%29",
            "~": "%7E",
            "%20": "+",
            "%00": "\0"
        };
        return encodeURIComponent(t).replace(/[!'()~]|%20|%00/g, function(i) {
            return e[i]
        })
    }
    function ba(t, e) {
        this._pairs = [],
        t && $r(t, this, e)
    }
    const ef = ba.prototype;
    ef.append = function(e, n) {
        this._pairs.push([e, n])
    }
    ;
    ef.toString = function(e) {
        const n = e ? function(i) {
            return e.call(this, i, wl)
        }
        : wl;
        return this._pairs.map(function(s) {
            return n(s[0]) + "=" + n(s[1])
        }, "").join("&")
    }
    ;
    function cm(t) {
        return encodeURIComponent(t).replace(/%3A/gi, ":").replace(/%24/g, "$").replace(/%2C/gi, ",").replace(/%20/g, "+").replace(/%5B/gi, "[").replace(/%5D/gi, "]")
    }
    function tf(t, e, n) {
        if (!e)
            return t;
        const i = n && n.encode || cm;
        R.isFunction(n) && (n = {
            serialize: n
        });
        const s = n && n.serialize;
        let r;
        if (s ? r = s(e, n) : r = R.isURLSearchParams(e) ? e.toString() : new ba(e,n).toString(i),
        r) {
            const o = t.indexOf("#");
            o !== -1 && (t = t.slice(0, o)),
            t += (t.indexOf("?") === -1 ? "?" : "&") + r
        }
        return t
    }
    class Sl {
        constructor() {
            this.handlers = []
        }
        use(e, n, i) {
            return this.handlers.push({
                fulfilled: e,
                rejected: n,
                synchronous: i ? i.synchronous : !1,
                runWhen: i ? i.runWhen : null
            }),
            this.handlers.length - 1
        }
        eject(e) {
            this.handlers[e] && (this.handlers[e] = null)
        }
        clear() {
            this.handlers && (this.handlers = [])
        }
        forEach(e) {
            R.forEach(this.handlers, function(i) {
                i !== null && e(i)
            })
        }
    }
    const nf = {
        silentJSONParsing: !0,
        forcedJSONParsing: !0,
        clarifyTimeoutError: !1
    }
      , um = typeof URLSearchParams != "undefined" ? URLSearchParams : ba
      , fm = typeof FormData != "undefined" ? FormData : null
      , dm = typeof Blob != "undefined" ? Blob : null
      , pm = {
        isBrowser: !0,
        classes: {
            URLSearchParams: um,
            FormData: fm,
            Blob: dm
        },
        protocols: ["http", "https", "file", "blob", "url", "data"]
    }
      , ya = typeof window != "undefined" && typeof document != "undefined"
      , Ao = typeof navigator == "object" && navigator || void 0
      , _m = ya && (!Ao || ["ReactNative", "NativeScript", "NS"].indexOf(Ao.product) < 0)
      , hm = typeof WorkerGlobalScope != "undefined" && self instanceof WorkerGlobalScope && typeof self.importScripts == "function"
      , mm = ya && window.location.href || "http://localhost"
      , gm = Object.freeze(Object.defineProperty({
        __proto__: null,
        hasBrowserEnv: ya,
        hasStandardBrowserEnv: _m,
        hasStandardBrowserWebWorkerEnv: hm,
        navigator: Ao,
        origin: mm
    }, Symbol.toStringTag, {
        value: "Module"
    }))
      , Ze = Bn(Bn({}, gm), pm);
    function bm(t, e) {
        return $r(t, new Ze.classes.URLSearchParams, Object.assign({
            visitor: function(n, i, s, r) {
                return Ze.isNode && R.isBuffer(n) ? (this.append(i, n.toString("base64")),
                !1) : r.defaultVisitor.apply(this, arguments)
            }
        }, e))
    }
    function ym(t) {
        return R.matchAll(/\w+|\[(\w*)]/g, t).map(e => e[0] === "[]" ? "" : e[1] || e[0])
    }
    function vm(t) {
        const e = {}
          , n = Object.keys(t);
        let i;
        const s = n.length;
        let r;
        for (i = 0; i < s; i++)
            r = n[i],
            e[r] = t[r];
        return e
    }
    function sf(t) {
        function e(n, i, s, r) {
            let o = n[r++];
            if (o === "__proto__")
                return !0;
            const a = Number.isFinite(+o)
              , l = r >= n.length;
            return o = !o && R.isArray(s) ? s.length : o,
            l ? (R.hasOwnProp(s, o) ? s[o] = [s[o], i] : s[o] = i,
            !a) : ((!s[o] || !R.isObject(s[o])) && (s[o] = []),
            e(n, i, s[o], r) && R.isArray(s[o]) && (s[o] = vm(s[o])),
            !a)
        }
        if (R.isFormData(t) && R.isFunction(t.entries)) {
            const n = {};
            return R.forEachEntry(t, (i, s) => {
                e(ym(i), s, n, 0)
            }
            ),
            n
        }
        return null
    }
    function km(t, e, n) {
        if (R.isString(t))
            try {
                return (e || JSON.parse)(t),
                R.trim(t)
            } catch (i) {
                if (i.name !== "SyntaxError")
                    throw i
            }
        return (n || JSON.stringify)(t)
    }
    const Os = {
        transitional: nf,
        adapter: ["xhr", "http", "fetch"],
        transformRequest: [function(e, n) {
            const i = n.getContentType() || ""
              , s = i.indexOf("application/json") > -1
              , r = R.isObject(e);
            if (r && R.isHTMLForm(e) && (e = new FormData(e)),
            R.isFormData(e))
                return s ? JSON.stringify(sf(e)) : e;
            if (R.isArrayBuffer(e) || R.isBuffer(e) || R.isStream(e) || R.isFile(e) || R.isBlob(e) || R.isReadableStream(e))
                return e;
            if (R.isArrayBufferView(e))
                return e.buffer;
            if (R.isURLSearchParams(e))
                return n.setContentType("application/x-www-form-urlencoded;charset=utf-8", !1),
                e.toString();
            let a;
            if (r) {
                if (i.indexOf("application/x-www-form-urlencoded") > -1)
                    return bm(e, this.formSerializer).toString();
                if ((a = R.isFileList(e)) || i.indexOf("multipart/form-data") > -1) {
                    const l = this.env && this.env.FormData;
                    return $r(a ? {
                        "files[]": e
                    } : e, l && new l, this.formSerializer)
                }
            }
            return r || s ? (n.setContentType("application/json", !1),
            km(e)) : e
        }
        ],
        transformResponse: [function(e) {
            const n = this.transitional || Os.transitional
              , i = n && n.forcedJSONParsing
              , s = this.responseType === "json";
            if (R.isResponse(e) || R.isReadableStream(e))
                return e;
            if (e && R.isString(e) && (i && !this.responseType || s)) {
                const o = !(n && n.silentJSONParsing) && s;
                try {
                    return JSON.parse(e)
                } catch (a) {
                    if (o)
                        throw a.name === "SyntaxError" ? se.from(a, se.ERR_BAD_RESPONSE, this, null, this.response) : a
                }
            }
            return e
        }
        ],
        timeout: 0,
        xsrfCookieName: "XSRF-TOKEN",
        xsrfHeaderName: "X-XSRF-TOKEN",
        maxContentLength: -1,
        maxBodyLength: -1,
        env: {
            FormData: Ze.classes.FormData,
            Blob: Ze.classes.Blob
        },
        validateStatus: function(e) {
            return e >= 200 && e < 300
        },
        headers: {
            common: {
                Accept: "application/json, text/plain, */*",
                "Content-Type": void 0
            }
        }
    };
    R.forEach(["delete", "get", "head", "post", "put", "patch"], t => {
        Os.headers[t] = {}
    }
    );
    const Am = R.toObjectSet(["age", "authorization", "content-length", "content-type", "etag", "expires", "from", "host", "if-modified-since", "if-unmodified-since", "last-modified", "location", "max-forwards", "proxy-authorization", "referer", "retry-after", "user-agent"])
      , $m = t => {
        const e = {};
        let n, i, s;
        return t && t.split("\n").forEach(function(o) {
            s = o.indexOf(":"),
            n = o.substring(0, s).trim().toLowerCase(),
            i = o.substring(s + 1).trim(),
            !(!n || e[n] && Am[n]) && (n === "set-cookie" ? e[n] ? e[n].push(i) : e[n] = [i] : e[n] = e[n] ? e[n] + ", " + i : i)
        }),
        e
    }
      , Ol = Symbol("internals");
    function zi(t) {
        return t && String(t).trim().toLowerCase()
    }
    function Bs(t) {
        return t === !1 || t == null ? t : R.isArray(t) ? t.map(Bs) : String(t)
    }
    function Tm(t) {
        const e = Object.create(null)
          , n = /([^\s,;=]+)\s*(?:=\s*([^,;]+))?/g;
        let i;
        for (; i = n.exec(t); )
            e[i[1]] = i[2];
        return e
    }
    const Em = t => /^[-_a-zA-Z0-9^`|~,!#$%&'*+.]+$/.test(t.trim());
    function Wr(t, e, n, i, s) {
        if (R.isFunction(i))
            return i.call(this, e, n);
        if (s && (e = n),
        !!R.isString(e)) {
            if (R.isString(i))
                return e.indexOf(i) !== -1;
            if (R.isRegExp(i))
                return i.test(e)
        }
    }
    function wm(t) {
        return t.trim().toLowerCase().replace(/([a-z\d])(\w*)/g, (e, n, i) => n.toUpperCase() + i)
    }
    function Sm(t, e) {
        const n = R.toCamelCase(" " + e);
        ["get", "set", "has"].forEach(i => {
            Object.defineProperty(t, i + n, {
                value: function(s, r, o) {
                    return this[i].call(this, e, s, r, o)
                },
                configurable: !0
            })
        }
        )
    }
    let _t = class {
        constructor(e) {
            e && this.set(e)
        }
        set(e, n, i) {
            const s = this;
            function r(a, l, c) {
                const u = zi(l);
                if (!u)
                    throw new Error("header name must be a non-empty string");
                const f = R.findKey(s, u);
                (!f || s[f] === void 0 || c === !0 || c === void 0 && s[f] !== !1) && (s[f || l] = Bs(a))
            }
            const o = (a, l) => R.forEach(a, (c, u) => r(c, u, l));
            if (R.isPlainObject(e) || e instanceof this.constructor)
                o(e, n);
            else if (R.isString(e) && (e = e.trim()) && !Em(e))
                o($m(e), n);
            else if (R.isHeaders(e))
                for (const [a,l] of e.entries())
                    r(l, a, i);
            else
                e != null && r(n, e, i);
            return this
        }
        get(e, n) {
            if (e = zi(e),
            e) {
                const i = R.findKey(this, e);
                if (i) {
                    const s = this[i];
                    if (!n)
                        return s;
                    if (n === !0)
                        return Tm(s);
                    if (R.isFunction(n))
                        return n.call(this, s, i);
                    if (R.isRegExp(n))
                        return n.exec(s);
                    throw new TypeError("parser must be boolean|regexp|function")
                }
            }
        }
        has(e, n) {
            if (e = zi(e),
            e) {
                const i = R.findKey(this, e);
                return !!(i && this[i] !== void 0 && (!n || Wr(this, this[i], i, n)))
            }
            return !1
        }
        delete(e, n) {
            const i = this;
            let s = !1;
            function r(o) {
                if (o = zi(o),
                o) {
                    const a = R.findKey(i, o);
                    a && (!n || Wr(i, i[a], a, n)) && (delete i[a],
                    s = !0)
                }
            }
            return R.isArray(e) ? e.forEach(r) : r(e),
            s
        }
        clear(e) {
            const n = Object.keys(this);
            let i = n.length
              , s = !1;
            for (; i--; ) {
                const r = n[i];
                (!e || Wr(this, this[r], r, e, !0)) && (delete this[r],
                s = !0)
            }
            return s
        }
        normalize(e) {
            const n = this
              , i = {};
            return R.forEach(this, (s, r) => {
                const o = R.findKey(i, r);
                if (o) {
                    n[o] = Bs(s),
                    delete n[r];
                    return
                }
                const a = e ? wm(r) : String(r).trim();
                a !== r && delete n[r],
                n[a] = Bs(s),
                i[a] = !0
            }
            ),
            this
        }
        concat(...e) {
            return this.constructor.concat(this, ...e)
        }
        toJSON(e) {
            const n = Object.create(null);
            return R.forEach(this, (i, s) => {
                i != null && i !== !1 && (n[s] = e && R.isArray(i) ? i.join(", ") : i)
            }
            ),
            n
        }
        [Symbol.iterator]() {
            return Object.entries(this.toJSON())[Symbol.iterator]()
        }
        toString() {
            return Object.entries(this.toJSON()).map( ([e,n]) => e + ": " + n).join("\n")
        }
        get[Symbol.toStringTag]() {
            return "AxiosHeaders"
        }
        static from(e) {
            return e instanceof this ? e : new this(e)
        }
        static concat(e, ...n) {
            const i = new this(e);
            return n.forEach(s => i.set(s)),
            i
        }
        static accessor(e) {
            const i = (this[Ol] = this[Ol] = {
                accessors: {}
            }).accessors
              , s = this.prototype;
            function r(o) {
                const a = zi(o);
                i[a] || (Sm(s, o),
                i[a] = !0)
            }
            return R.isArray(e) ? e.forEach(r) : r(e),
            this
        }
    }
    ;
    _t.accessor(["Content-Type", "Content-Length", "Accept", "Accept-Encoding", "User-Agent", "Authorization"]);
    R.reduceDescriptors(_t.prototype, ({value: t}, e) => {
        let n = e[0].toUpperCase() + e.slice(1);
        return {
            get: () => t,
            set(i) {
                this[n] = i
            }
        }
    }
    );
    R.freezeMethods(_t);
    function Yr(t, e) {
        const n = this || Os
          , i = e || n
          , s = _t.from(i.headers);
        let r = i.data;
        return R.forEach(t, function(a) {
            r = a.call(n, r, s.normalize(), e ? e.status : void 0)
        }),
        s.normalize(),
        r
    }
    function rf(t) {
        return !!(t && t.__CANCEL__)
    }
    function Gi(t, e, n) {
        se.call(this, t == null ? "canceled" : t, se.ERR_CANCELED, e, n),
        this.name = "CanceledError"
    }
    R.inherits(Gi, se, {
        __CANCEL__: !0
    });
    function of(t, e, n) {
        const i = n.config.validateStatus;
        !n.status || !i || i(n.status) ? t(n) : e(new se("Request failed with status code " + n.status,[se.ERR_BAD_REQUEST, se.ERR_BAD_RESPONSE][Math.floor(n.status / 100) - 4],n.config,n.request,n))
    }
    function Om(t) {
        const e = /^([-+\w]{1,25})(:?\/\/|:)/.exec(t);
        return e && e[1] || ""
    }
    function Pm(t, e) {
        t = t || 10;
        const n = new Array(t)
          , i = new Array(t);
        let s = 0, r = 0, o;
        return e = e !== void 0 ? e : 1e3,
        function(l) {
            const c = Date.now()
              , u = i[r];
            o || (o = c),
            n[s] = l,
            i[s] = c;
            let f = r
              , d = 0;
            for (; f !== s; )
                d += n[f++],
                f = f % t;
            if (s = (s + 1) % t,
            s === r && (r = (r + 1) % t),
            c - o < e)
                return;
            const _ = u && c - u;
            return _ ? Math.round(d * 1e3 / _) : void 0
        }
    }
    function xm(t, e) {
        let n = 0, i = 1e3 / e, s, r;
        const o = (c, u=Date.now()) => {
            n = u,
            s = null,
            r && (clearTimeout(r),
            r = null),
            t.apply(null, c)
        }
        ;
        return [ (...c) => {
            const u = Date.now()
              , f = u - n;
            f >= i ? o(c, u) : (s = c,
            r || (r = setTimeout( () => {
                r = null,
                o(s)
            }
            , i - f)))
        }
        , () => s && o(s)]
    }
    const nr = (t, e, n=3) => {
        let i = 0;
        const s = Pm(50, 250);
        return xm(r => {
            const o = r.loaded
              , a = r.lengthComputable ? r.total : void 0
              , l = o - i
              , c = s(l)
              , u = o <= a;
            i = o;
            const f = {
                loaded: o,
                total: a,
                progress: a ? o / a : void 0,
                bytes: l,
                rate: c || void 0,
                estimated: c && a && u ? (a - o) / c : void 0,
                event: r,
                lengthComputable: a != null,
                [e ? "download" : "upload"]: !0
            };
            t(f)
        }
        , n)
    }
      , Pl = (t, e) => {
        const n = t != null;
        return [i => e[0]({
            lengthComputable: n,
            total: t,
            loaded: i
        }), e[1]]
    }
      , xl = t => (...e) => R.asap( () => t(...e))
      , Im = Ze.hasStandardBrowserEnv ? ( (t, e) => n => (n = new URL(n,Ze.origin),
    t.protocol === n.protocol && t.host === n.host && (e || t.port === n.port)))(new URL(Ze.origin), Ze.navigator && /(msie|trident)/i.test(Ze.navigator.userAgent)) : () => !0
      , Cm = Ze.hasStandardBrowserEnv ? {
        write(t, e, n, i, s, r) {
            const o = [t + "=" + encodeURIComponent(e)];
            R.isNumber(n) && o.push("expires=" + new Date(n).toGMTString()),
            R.isString(i) && o.push("path=" + i),
            R.isString(s) && o.push("domain=" + s),
            r === !0 && o.push("secure"),
            document.cookie = o.join("; ")
        },
        read(t) {
            const e = document.cookie.match(new RegExp("(^|;\\s*)(" + t + ")=([^;]*)"));
            return e ? decodeURIComponent(e[3]) : null
        },
        remove(t) {
            this.write(t, "", Date.now() - 864e5)
        }
    } : {
        write() {},
        read() {
            return null
        },
        remove() {}
    };
    function Rm(t) {
        return /^([a-z][a-z\d+\-.]*:)?\/\//i.test(t)
    }
    function Lm(t, e) {
        return e ? t.replace(/\/?\/$/, "") + "/" + e.replace(/^\/+/, "") : t
    }
    function af(t, e) {
        return t && !Rm(e) ? Lm(t, e) : e
    }
    const Il = t => t instanceof _t ? Bn({}, t) : t;
    function li(t, e) {
        e = e || {};
        const n = {};
        function i(c, u, f, d) {
            return R.isPlainObject(c) && R.isPlainObject(u) ? R.merge.call({
                caseless: d
            }, c, u) : R.isPlainObject(u) ? R.merge({}, u) : R.isArray(u) ? u.slice() : u
        }
        function s(c, u, f, d) {
            if (R.isUndefined(u)) {
                if (!R.isUndefined(c))
                    return i(void 0, c, f, d)
            } else
                return i(c, u, f, d)
        }
        function r(c, u) {
            if (!R.isUndefined(u))
                return i(void 0, u)
        }
        function o(c, u) {
            if (R.isUndefined(u)) {
                if (!R.isUndefined(c))
                    return i(void 0, c)
            } else
                return i(void 0, u)
        }
        function a(c, u, f) {
            if (f in e)
                return i(c, u);
            if (f in t)
                return i(void 0, c)
        }
        const l = {
            url: r,
            method: r,
            data: r,
            baseURL: o,
            transformRequest: o,
            transformResponse: o,
            paramsSerializer: o,
            timeout: o,
            timeoutMessage: o,
            withCredentials: o,
            withXSRFToken: o,
            adapter: o,
            responseType: o,
            xsrfCookieName: o,
            xsrfHeaderName: o,
            onUploadProgress: o,
            onDownloadProgress: o,
            decompress: o,
            maxContentLength: o,
            maxBodyLength: o,
            beforeRedirect: o,
            transport: o,
            httpAgent: o,
            httpsAgent: o,
            cancelToken: o,
            socketPath: o,
            responseEncoding: o,
            validateStatus: a,
            headers: (c, u, f) => s(Il(c), Il(u), f, !0)
        };
        return R.forEach(Object.keys(Object.assign({}, t, e)), function(u) {
            const f = l[u] || s
              , d = f(t[u], e[u], u);
            R.isUndefined(d) && f !== a || (n[u] = d)
        }),
        n
    }
    const lf = t => {
        const e = li({}, t);
        let {data: n, withXSRFToken: i, xsrfHeaderName: s, xsrfCookieName: r, headers: o, auth: a} = e;
        e.headers = o = _t.from(o),
        e.url = tf(af(e.baseURL, e.url), t.params, t.paramsSerializer),
        a && o.set("Authorization", "Basic " + btoa((a.username || "") + ":" + (a.password ? unescape(encodeURIComponent(a.password)) : "")));
        let l;
        if (R.isFormData(n)) {
            if (Ze.hasStandardBrowserEnv || Ze.hasStandardBrowserWebWorkerEnv)
                o.setContentType(void 0);
            else if ((l = o.getContentType()) !== !1) {
                const [c,...u] = l ? l.split(";").map(f => f.trim()).filter(Boolean) : [];
                o.setContentType([c || "multipart/form-data", ...u].join("; "))
            }
        }
        if (Ze.hasStandardBrowserEnv && (i && R.isFunction(i) && (i = i(e)),
        i || i !== !1 && Im(e.url))) {
            const c = s && r && Cm.read(r);
            c && o.set(s, c)
        }
        return e
    }
      , Nm = typeof XMLHttpRequest != "undefined"
      , Dm = Nm && function(t) {
        return new Promise(function(n, i) {
            const s = lf(t);
            let r = s.data;
            const o = _t.from(s.headers).normalize();
            let {responseType: a, onUploadProgress: l, onDownloadProgress: c} = s, u, f, d, _, m;
            function p() {
                _ && _(),
                m && m(),
                s.cancelToken && s.cancelToken.unsubscribe(u),
                s.signal && s.signal.removeEventListener("abort", u)
            }
            let b = new XMLHttpRequest;
            b.open(s.method.toUpperCase(), s.url, !0),
            b.timeout = s.timeout;
            function E() {
                if (!b)
                    return;
                const v = _t.from("getAllResponseHeaders"in b && b.getAllResponseHeaders())
                  , A = {
                    data: !a || a === "text" || a === "json" ? b.responseText : b.response,
                    status: b.status,
                    statusText: b.statusText,
                    headers: v,
                    config: t,
                    request: b
                };
                of(function(O) {
                    n(O),
                    p()
                }, function(O) {
                    i(O),
                    p()
                }, A),
                b = null
            }
            "onloadend"in b ? b.onloadend = E : b.onreadystatechange = function() {
                !b || b.readyState !== 4 || b.status === 0 && !(b.responseURL && b.responseURL.indexOf("file:") === 0) || setTimeout(E)
            }
            ,
            b.onabort = function() {
                b && (i(new se("Request aborted",se.ECONNABORTED,t,b)),
                b = null)
            }
            ,
            b.onerror = function() {
                i(new se("Network Error",se.ERR_NETWORK,t,b)),
                b = null
            }
            ,
            b.ontimeout = function() {
                let y = s.timeout ? "timeout of " + s.timeout + "ms exceeded" : "timeout exceeded";
                const A = s.transitional || nf;
                s.timeoutErrorMessage && (y = s.timeoutErrorMessage),
                i(new se(y,A.clarifyTimeoutError ? se.ETIMEDOUT : se.ECONNABORTED,t,b)),
                b = null
            }
            ,
            r === void 0 && o.setContentType(null),
            "setRequestHeader"in b && R.forEach(o.toJSON(), function(y, A) {
                b.setRequestHeader(A, y)
            }),
            R.isUndefined(s.withCredentials) || (b.withCredentials = !!s.withCredentials),
            a && a !== "json" && (b.responseType = s.responseType),
            c && ([d,m] = nr(c, !0),
            b.addEventListener("progress", d)),
            l && b.upload && ([f,_] = nr(l),
            b.upload.addEventListener("progress", f),
            b.upload.addEventListener("loadend", _)),
            (s.cancelToken || s.signal) && (u = v => {
                b && (i(!v || v.type ? new Gi(null,t,b) : v),
                b.abort(),
                b = null)
            }
            ,
            s.cancelToken && s.cancelToken.subscribe(u),
            s.signal && (s.signal.aborted ? u() : s.signal.addEventListener("abort", u)));
            const S = Om(s.url);
            if (S && Ze.protocols.indexOf(S) === -1) {
                i(new se("Unsupported protocol " + S + ":",se.ERR_BAD_REQUEST,t));
                return
            }
            b.send(r || null)
        }
        )
    }
      , Mm = (t, e) => {
        const {length: n} = t = t ? t.filter(Boolean) : [];
        if (e || n) {
            let i = new AbortController, s;
            const r = function(c) {
                if (!s) {
                    s = !0,
                    a();
                    const u = c instanceof Error ? c : this.reason;
                    i.abort(u instanceof se ? u : new Gi(u instanceof Error ? u.message : u))
                }
            };
            let o = e && setTimeout( () => {
                o = null,
                r(new se("timeout ".concat(e, " of ms exceeded"),se.ETIMEDOUT))
            }
            , e);
            const a = () => {
                t && (o && clearTimeout(o),
                o = null,
                t.forEach(c => {
                    c.unsubscribe ? c.unsubscribe(r) : c.removeEventListener("abort", r)
                }
                ),
                t = null)
            }
            ;
            t.forEach(c => c.addEventListener("abort", r));
            const {signal: l} = i;
            return l.unsubscribe = () => R.asap(a),
            l
        }
    }
      , Fm = function*(t, e) {
        let n = t.byteLength;
        if (n < e) {
            yield t;
            return
        }
        let i = 0, s;
        for (; i < n; )
            s = i + e,
            yield t.slice(i, s),
            i = s
    }
      , Gm = function(t, e) {
        return Nr(this, null, function*() {
            try {
                for (var n = qa(Um(t)), i, s, r; i = !(s = yield new zn(n.next())).done; i = !1) {
                    const o = s.value;
                    yield*Dr(Fm(o, e))
                }
            } catch (s) {
                r = [s]
            } finally {
                try {
                    i && (s = n.return) && (yield new zn(s.call(n)))
                } finally {
                    if (r)
                        throw r[0]
                }
            }
        })
    }
      , Um = function(t) {
        return Nr(this, null, function*() {
            if (t[Symbol.asyncIterator]) {
                yield*Dr(t);
                return
            }
            const e = t.getReader();
            try {
                for (; ; ) {
                    const {done: n, value: i} = yield new zn(e.read());
                    if (n)
                        break;
                    yield i
                }
            } finally {
                yield new zn(e.cancel())
            }
        })
    }
      , Cl = (t, e, n, i) => {
        const s = Gm(t, e);
        let r = 0, o, a = c => {
            o || (o = !0,
            i && i(c))
        }
        ;
        return new ReadableStream({
            pull(c) {
                return At(this, null, function*() {
                    try {
                        const {done: u, value: f} = yield s.next();
                        if (u) {
                            a(),
                            c.close();
                            return
                        }
                        let d = f.byteLength;
                        if (n) {
                            let _ = r += d;
                            n(_)
                        }
                        c.enqueue(new Uint8Array(f))
                    } catch (u) {
                        throw a(u),
                        u
                    }
                })
            },
            cancel(c) {
                return a(c),
                s.return()
            }
        },{
            highWaterMark: 2
        })
    }
      , Tr = typeof fetch == "function" && typeof Request == "function" && typeof Response == "function"
      , cf = Tr && typeof ReadableStream == "function"
      , jm = Tr && (typeof TextEncoder == "function" ? (t => e => t.encode(e))(new TextEncoder) : t => At(xs, null, function*() {
        return new Uint8Array(yield new Response(t).arrayBuffer())
    }))
      , uf = (t, ...e) => {
        try {
            return !!t(...e)
        } catch (n) {
            return !1
        }
    }
      , Bm = cf && uf( () => {
        let t = !1;
        const e = new Request(Ze.origin,{
            body: new ReadableStream,
            method: "POST",
            get duplex() {
                return t = !0,
                "half"
            }
        }).headers.has("Content-Type");
        return t && !e
    }
    )
      , Rl = 64 * 1024
      , $o = cf && uf( () => R.isReadableStream(new Response("").body))
      , ir = {
        stream: $o && (t => t.body)
    };
    Tr && (t => {
        ["text", "arrayBuffer", "blob", "formData", "stream"].forEach(e => {
            !ir[e] && (ir[e] = R.isFunction(t[e]) ? n => n[e]() : (n, i) => {
                throw new se("Response type '".concat(e, "' is not supported"),se.ERR_NOT_SUPPORT,i)
            }
            )
        }
        )
    }
    )(new Response);
    const zm = t => At(xs, null, function*() {
        if (t == null)
            return 0;
        if (R.isBlob(t))
            return t.size;
        if (R.isSpecCompliantForm(t))
            return (yield new Request(Ze.origin,{
                method: "POST",
                body: t
            }).arrayBuffer()).byteLength;
        if (R.isArrayBufferView(t) || R.isArrayBuffer(t))
            return t.byteLength;
        if (R.isURLSearchParams(t) && (t = t + ""),
        R.isString(t))
            return (yield jm(t)).byteLength
    })
      , Vm = (t, e) => At(xs, null, function*() {
        const n = R.toFiniteNumber(t.getContentLength());
        return n == null ? zm(e) : n
    })
      , Hm = Tr && (t => At(xs, null, function*() {
        let {url: e, method: n, data: i, signal: s, cancelToken: r, timeout: o, onDownloadProgress: a, onUploadProgress: l, responseType: c, headers: u, withCredentials: f="same-origin", fetchOptions: d} = lf(t);
        c = c ? (c + "").toLowerCase() : "text";
        let _ = Mm([s, r && r.toAbortSignal()], o), m;
        const p = _ && _.unsubscribe && ( () => {
            _.unsubscribe()
        }
        );
        let b;
        try {
            if (l && Bm && n !== "get" && n !== "head" && (b = yield Vm(u, i)) !== 0) {
                let A = new Request(e,{
                    method: "POST",
                    body: i,
                    duplex: "half"
                }), T;
                if (R.isFormData(i) && (T = A.headers.get("content-type")) && u.setContentType(T),
                A.body) {
                    const [O,C] = Pl(b, nr(xl(l)));
                    i = Cl(A.body, Rl, O, C)
                }
            }
            R.isString(f) || (f = f ? "include" : "omit");
            const E = "credentials"in Request.prototype;
            m = new Request(e,Is(Bn({}, d), {
                signal: _,
                method: n.toUpperCase(),
                headers: u.normalize().toJSON(),
                body: i,
                duplex: "half",
                credentials: E ? f : void 0
            }));
            let S = yield fetch(m);
            const v = $o && (c === "stream" || c === "response");
            if ($o && (a || v && p)) {
                const A = {};
                ["status", "statusText", "headers"].forEach(P => {
                    A[P] = S[P]
                }
                );
                const T = R.toFiniteNumber(S.headers.get("content-length"))
                  , [O,C] = a && Pl(T, nr(xl(a), !0)) || [];
                S = new Response(Cl(S.body, Rl, O, () => {
                    C && C(),
                    p && p()
                }
                ),A)
            }
            c = c || "text";
            let y = yield ir[R.findKey(ir, c) || "text"](S, t);
            return !v && p && p(),
            yield new Promise( (A, T) => {
                of(A, T, {
                    data: y,
                    headers: _t.from(S.headers),
                    status: S.status,
                    statusText: S.statusText,
                    config: t,
                    request: m
                })
            }
            )
        } catch (E) {
            throw p && p(),
            E && E.name === "TypeError" && /fetch/i.test(E.message) ? Object.assign(new se("Network Error",se.ERR_NETWORK,t,m), {
                cause: E.cause || E
            }) : se.from(E, E && E.code, t, m)
        }
    }))
      , To = {
        http: om,
        xhr: Dm,
        fetch: Hm
    };
    R.forEach(To, (t, e) => {
        if (t) {
            try {
                Object.defineProperty(t, "name", {
                    value: e
                })
            } catch (n) {}
            Object.defineProperty(t, "adapterName", {
                value: e
            })
        }
    }
    );
    const Ll = t => "- ".concat(t)
      , Km = t => R.isFunction(t) || t === null || t === !1
      , ff = {
        getAdapter: t => {
            t = R.isArray(t) ? t : [t];
            const {length: e} = t;
            let n, i;
            const s = {};
            for (let r = 0; r < e; r++) {
                n = t[r];
                let o;
                if (i = n,
                !Km(n) && (i = To[(o = String(n)).toLowerCase()],
                i === void 0))
                    throw new se("Unknown adapter '".concat(o, "'"));
                if (i)
                    break;
                s[o || "#" + r] = i
            }
            if (!i) {
                const r = Object.entries(s).map( ([a,l]) => "adapter ".concat(a, " ") + (l === !1 ? "is not supported by the environment" : "is not available in the build"));
                let o = e ? r.length > 1 ? "since :\n" + r.map(Ll).join("\n") : " " + Ll(r[0]) : "as no adapter specified";
                throw new se("There is no suitable adapter to dispatch the request " + o,"ERR_NOT_SUPPORT")
            }
            return i
        }
        ,
        adapters: To
    };
    function Xr(t) {
        if (t.cancelToken && t.cancelToken.throwIfRequested(),
        t.signal && t.signal.aborted)
            throw new Gi(null,t)
    }
    function Nl(t) {
        return Xr(t),
        t.headers = _t.from(t.headers),
        t.data = Yr.call(t, t.transformRequest),
        ["post", "put", "patch"].indexOf(t.method) !== -1 && t.headers.setContentType("application/x-www-form-urlencoded", !1),
        ff.getAdapter(t.adapter || Os.adapter)(t).then(function(i) {
            return Xr(t),
            i.data = Yr.call(t, t.transformResponse, i),
            i.headers = _t.from(i.headers),
            i
        }, function(i) {
            return rf(i) || (Xr(t),
            i && i.response && (i.response.data = Yr.call(t, t.transformResponse, i.response),
            i.response.headers = _t.from(i.response.headers))),
            Promise.reject(i)
        })
    }
    const df = "1.7.9"
      , Er = {};
    ["object", "boolean", "number", "function", "string", "symbol"].forEach( (t, e) => {
        Er[t] = function(i) {
            return typeof i === t || "a" + (e < 1 ? "n " : " ") + t
        }
    }
    );
    const Dl = {};
    Er.transitional = function(e, n, i) {
        function s(r, o) {
            return "[Axios v" + df + "] Transitional option '" + r + "'" + o + (i ? ". " + i : "")
        }
        return (r, o, a) => {
            if (e === !1)
                throw new se(s(o, " has been removed" + (n ? " in " + n : "")),se.ERR_DEPRECATED);
            return n && !Dl[o] && (Dl[o] = !0,
            console.warn(s(o, " has been deprecated since v" + n + " and will be removed in the near future"))),
            e ? e(r, o, a) : !0
        }
    }
    ;
    Er.spelling = function(e) {
        return (n, i) => (console.warn("".concat(i, " is likely a misspelling of ").concat(e)),
        !0)
    }
    ;
    function qm(t, e, n) {
        if (typeof t != "object")
            throw new se("options must be an object",se.ERR_BAD_OPTION_VALUE);
        const i = Object.keys(t);
        let s = i.length;
        for (; s-- > 0; ) {
            const r = i[s]
              , o = e[r];
            if (o) {
                const a = t[r]
                  , l = a === void 0 || o(a, r, t);
                if (l !== !0)
                    throw new se("option " + r + " must be " + l,se.ERR_BAD_OPTION_VALUE);
                continue
            }
            if (n !== !0)
                throw new se("Unknown option " + r,se.ERR_BAD_OPTION)
        }
    }
    const zs = {
        assertOptions: qm,
        validators: Er
    }
      , qt = zs.validators;
    let ti = class {
        constructor(e) {
            this.defaults = e,
            this.interceptors = {
                request: new Sl,
                response: new Sl
            }
        }
        request(e, n) {
            return At(this, null, function*() {
                try {
                    return yield this._request(e, n)
                } catch (i) {
                    if (i instanceof Error) {
                        let s = {};
                        Error.captureStackTrace ? Error.captureStackTrace(s) : s = new Error;
                        const r = s.stack ? s.stack.replace(/^.+\n/, "") : "";
                        try {
                            i.stack ? r && !String(i.stack).endsWith(r.replace(/^.+\n.+\n/, "")) && (i.stack += "\n" + r) : i.stack = r
                        } catch (o) {}
                    }
                    throw i
                }
            })
        }
        _request(e, n) {
            typeof e == "string" ? (n = n || {},
            n.url = e) : n = e || {},
            n = li(this.defaults, n);
            const {transitional: i, paramsSerializer: s, headers: r} = n;
            i !== void 0 && zs.assertOptions(i, {
                silentJSONParsing: qt.transitional(qt.boolean),
                forcedJSONParsing: qt.transitional(qt.boolean),
                clarifyTimeoutError: qt.transitional(qt.boolean)
            }, !1),
            s != null && (R.isFunction(s) ? n.paramsSerializer = {
                serialize: s
            } : zs.assertOptions(s, {
                encode: qt.function,
                serialize: qt.function
            }, !0)),
            zs.assertOptions(n, {
                baseUrl: qt.spelling("baseURL"),
                withXsrfToken: qt.spelling("withXSRFToken")
            }, !0),
            n.method = (n.method || this.defaults.method || "get").toLowerCase();
            let o = r && R.merge(r.common, r[n.method]);
            r && R.forEach(["delete", "get", "head", "post", "put", "patch", "common"], m => {
                delete r[m]
            }
            ),
            n.headers = _t.concat(o, r);
            const a = [];
            let l = !0;
            this.interceptors.request.forEach(function(p) {
                typeof p.runWhen == "function" && p.runWhen(n) === !1 || (l = l && p.synchronous,
                a.unshift(p.fulfilled, p.rejected))
            });
            const c = [];
            this.interceptors.response.forEach(function(p) {
                c.push(p.fulfilled, p.rejected)
            });
            let u, f = 0, d;
            if (!l) {
                const m = [Nl.bind(this), void 0];
                for (m.unshift.apply(m, a),
                m.push.apply(m, c),
                d = m.length,
                u = Promise.resolve(n); f < d; )
                    u = u.then(m[f++], m[f++]);
                return u
            }
            d = a.length;
            let _ = n;
            for (f = 0; f < d; ) {
                const m = a[f++]
                  , p = a[f++];
                try {
                    _ = m(_)
                } catch (b) {
                    p.call(this, b);
                    break
                }
            }
            try {
                u = Nl.call(this, _)
            } catch (m) {
                return Promise.reject(m)
            }
            for (f = 0,
            d = c.length; f < d; )
                u = u.then(c[f++], c[f++]);
            return u
        }
        getUri(e) {
            e = li(this.defaults, e);
            const n = af(e.baseURL, e.url);
            return tf(n, e.params, e.paramsSerializer)
        }
    }
    ;
    R.forEach(["delete", "get", "head", "options"], function(e) {
        ti.prototype[e] = function(n, i) {
            return this.request(li(i || {}, {
                method: e,
                url: n,
                data: (i || {}).data
            }))
        }
    });
    R.forEach(["post", "put", "patch"], function(e) {
        function n(i) {
            return function(r, o, a) {
                return this.request(li(a || {}, {
                    method: e,
                    headers: i ? {
                        "Content-Type": "multipart/form-data"
                    } : {},
                    url: r,
                    data: o
                }))
            }
        }
        ti.prototype[e] = n(),
        ti.prototype[e + "Form"] = n(!0)
    });
    let Wm = class pf {
        constructor(e) {
            if (typeof e != "function")
                throw new TypeError("executor must be a function.");
            let n;
            this.promise = new Promise(function(r) {
                n = r
            }
            );
            const i = this;
            this.promise.then(s => {
                if (!i._listeners)
                    return;
                let r = i._listeners.length;
                for (; r-- > 0; )
                    i._listeners[r](s);
                i._listeners = null
            }
            ),
            this.promise.then = s => {
                let r;
                const o = new Promise(a => {
                    i.subscribe(a),
                    r = a
                }
                ).then(s);
                return o.cancel = function() {
                    i.unsubscribe(r)
                }
                ,
                o
            }
            ,
            e(function(r, o, a) {
                i.reason || (i.reason = new Gi(r,o,a),
                n(i.reason))
            })
        }
        throwIfRequested() {
            if (this.reason)
                throw this.reason
        }
        subscribe(e) {
            if (this.reason) {
                e(this.reason);
                return
            }
            this._listeners ? this._listeners.push(e) : this._listeners = [e]
        }
        unsubscribe(e) {
            if (!this._listeners)
                return;
            const n = this._listeners.indexOf(e);
            n !== -1 && this._listeners.splice(n, 1)
        }
        toAbortSignal() {
            const e = new AbortController
              , n = i => {
                e.abort(i)
            }
            ;
            return this.subscribe(n),
            e.signal.unsubscribe = () => this.unsubscribe(n),
            e.signal
        }
        static source() {
            let e;
            return {
                token: new pf(function(s) {
                    e = s
                }
                ),
                cancel: e
            }
        }
    }
    ;
    function Ym(t) {
        return function(n) {
            return t.apply(null, n)
        }
    }
    function Xm(t) {
        return R.isObject(t) && t.isAxiosError === !0
    }
    const Eo = {
        Continue: 100,
        SwitchingProtocols: 101,
        Processing: 102,
        EarlyHints: 103,
        Ok: 200,
        Created: 201,
        Accepted: 202,
        NonAuthoritativeInformation: 203,
        NoContent: 204,
        ResetContent: 205,
        PartialContent: 206,
        MultiStatus: 207,
        AlreadyReported: 208,
        ImUsed: 226,
        MultipleChoices: 300,
        MovedPermanently: 301,
        Found: 302,
        SeeOther: 303,
        NotModified: 304,
        UseProxy: 305,
        Unused: 306,
        TemporaryRedirect: 307,
        PermanentRedirect: 308,
        BadRequest: 400,
        Unauthorized: 401,
        PaymentRequired: 402,
        Forbidden: 403,
        NotFound: 404,
        MethodNotAllowed: 405,
        NotAcceptable: 406,
        ProxyAuthenticationRequired: 407,
        RequestTimeout: 408,
        Conflict: 409,
        Gone: 410,
        LengthRequired: 411,
        PreconditionFailed: 412,
        PayloadTooLarge: 413,
        UriTooLong: 414,
        UnsupportedMediaType: 415,
        RangeNotSatisfiable: 416,
        ExpectationFailed: 417,
        ImATeapot: 418,
        MisdirectedRequest: 421,
        UnprocessableEntity: 422,
        Locked: 423,
        FailedDependency: 424,
        TooEarly: 425,
        UpgradeRequired: 426,
        PreconditionRequired: 428,
        TooManyRequests: 429,
        RequestHeaderFieldsTooLarge: 431,
        UnavailableForLegalReasons: 451,
        InternalServerError: 500,
        NotImplemented: 501,
        BadGateway: 502,
        ServiceUnavailable: 503,
        GatewayTimeout: 504,
        HttpVersionNotSupported: 505,
        VariantAlsoNegotiates: 506,
        InsufficientStorage: 507,
        LoopDetected: 508,
        NotExtended: 510,
        NetworkAuthenticationRequired: 511
    };
    Object.entries(Eo).forEach( ([t,e]) => {
        Eo[e] = t
    }
    );
    function _f(t) {
        const e = new ti(t)
          , n = zu(ti.prototype.request, e);
        return R.extend(n, ti.prototype, e, {
            allOwnKeys: !0
        }),
        R.extend(n, e, null, {
            allOwnKeys: !0
        }),
        n.create = function(s) {
            return _f(li(t, s))
        }
        ,
        n
    }
    const Pe = _f(Os);
    Pe.Axios = ti;
    Pe.CanceledError = Gi;
    Pe.CancelToken = Wm;
    Pe.isCancel = rf;
    Pe.VERSION = df;
    Pe.toFormData = $r;
    Pe.AxiosError = se;
    Pe.Cancel = Pe.CanceledError;
    Pe.all = function(e) {
        return Promise.all(e)
    }
    ;
    Pe.spread = Ym;
    Pe.isAxiosError = Xm;
    Pe.mergeConfig = li;
    Pe.AxiosHeaders = _t;
    Pe.formToJSON = t => sf(R.isHTMLForm(t) ? new FormData(t) : t);
    Pe.getAdapter = ff.getAdapter;
    Pe.HttpStatusCode = Eo;
    Pe.default = Pe;
    const {Axios: eB, AxiosError: tB, CanceledError: nB, isCancel: iB, CancelToken: sB, VERSION: rB, all: oB, Cancel: aB, isAxiosError: lB, spread: cB, toFormData: uB, AxiosHeaders: fB, HttpStatusCode: dB, formToJSON: pB, getAdapter: _B, mergeConfig: hB} = Pe;
    function dn(t) {
        if (t === void 0)
            throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
        return t
    }
    function hf(t, e) {
        t.prototype = Object.create(e.prototype),
        t.prototype.constructor = t,
        t.__proto__ = e
    }
    /*!
 * GSAP 3.12.7
 * https://gsap.com
 *
 * @license Copyright 2008-2025, GreenSock. All rights reserved.
 * Subject to the terms at https://gsap.com/standard-license or for
 * Club GSAP members, the agreement issued with that membership.
 * @author: Jack Doyle, jack@greensock.com
*/
    var xt = {
        autoSleep: 120,
        force3D: "auto",
        nullTargetWarn: 1,
        units: {
            lineHeight: ""
        }
    }, Pi = {
        duration: .5,
        overwrite: !1,
        delay: 0
    }, va, We, Ee, tn = 1e8, tt = 1 / tn, wo = Math.PI * 2, Jm = wo / 4, Qm = 0, mf = Math.sqrt, Zm = Math.cos, eg = Math.sin, He = function(e) {
        return typeof e == "string"
    }, Le = function(e) {
        return typeof e == "function"
    }, mn = function(e) {
        return typeof e == "number"
    }, ka = function(e) {
        return typeof e == "undefined"
    }, rn = function(e) {
        return typeof e == "object"
    }, ht = function(e) {
        return e !== !1
    }, Aa = function() {
        return typeof window != "undefined"
    }, Ns = function(e) {
        return Le(e) || He(e)
    }, gf = typeof ArrayBuffer == "function" && ArrayBuffer.isView || function() {}
    , nt = Array.isArray, So = /(?:-?\.?\d|\.)+/gi, bf = /[-+=.]*\d+[.e\-+]*\d*[e\-+]*\d*/g, bi = /[-+=.]*\d+[.e-]*\d*[a-z%]*/g, Jr = /[-+=.]*\d+\.?\d*(?:e-|e\+)?\d*/gi, yf = /[+-]=-?[.\d]+/, vf = /[^,'"\[\]\s]+/gi, tg = /^[+\-=e\s\d]*\d+[.\d]*([a-z]*|%)\s*$/i, Se, Yt, Oo, $a, It = {}, sr = {}, kf, Af = function(e) {
        return (sr = xi(e, It)) && yt
    }, Ta = function(e, n) {
        return console.warn("Invalid property", e, "set to", n, "Missing plugin? gsap.registerPlugin()")
    }, _s = function(e, n) {
        return !n && console.warn(e)
    }, $f = function(e, n) {
        return e && (It[e] = n) && sr && (sr[e] = n) || It
    }, hs = function() {
        return 0
    }, ng = {
        suppressEvents: !0,
        isStart: !0,
        kill: !1
    }, Vs = {
        suppressEvents: !0,
        kill: !1
    }, ig = {
        suppressEvents: !0
    }, Ea = {}, On = [], Po = {}, Tf, Et = {}, Qr = {}, Ml = 30, Hs = [], wa = "", Sa = function(e) {
        var n = e[0], i, s;
        if (rn(n) || Le(n) || (e = [e]),
        !(i = (n._gsap || {}).harness)) {
            for (s = Hs.length; s-- && !Hs[s].targetTest(n); )
                ;
            i = Hs[s]
        }
        for (s = e.length; s--; )
            e[s] && (e[s]._gsap || (e[s]._gsap = new Wf(e[s],i))) || e.splice(s, 1);
        return e
    }, ni = function(e) {
        return e._gsap || Sa(Nt(e))[0]._gsap
    }, Ef = function(e, n, i) {
        return (i = e[n]) && Le(i) ? e[n]() : ka(i) && e.getAttribute && e.getAttribute(n) || i
    }, mt = function(e, n) {
        return (e = e.split(",")).forEach(n) || e
    }, Ne = function(e) {
        return Math.round(e * 1e5) / 1e5 || 0
    }, Ge = function(e) {
        return Math.round(e * 1e7) / 1e7 || 0
    }, Ei = function(e, n) {
        var i = n.charAt(0)
          , s = parseFloat(n.substr(2));
        return e = parseFloat(e),
        i === "+" ? e + s : i === "-" ? e - s : i === "*" ? e * s : e / s
    }, sg = function(e, n) {
        for (var i = n.length, s = 0; e.indexOf(n[s]) < 0 && ++s < i; )
            ;
        return s < i
    }, rr = function() {
        var e = On.length, n = On.slice(0), i, s;
        for (Po = {},
        On.length = 0,
        i = 0; i < e; i++)
            s = n[i],
            s && s._lazy && (s.render(s._lazy[0], s._lazy[1], !0)._lazy = 0)
    }, wf = function(e, n, i, s) {
        On.length && !We && rr(),
        e.render(n, i, We && n < 0 && (e._initted || e._startAt)),
        On.length && !We && rr()
    }, Sf = function(e) {
        var n = parseFloat(e);
        return (n || n === 0) && (e + "").match(vf).length < 2 ? n : He(e) ? e.trim() : e
    }, Of = function(e) {
        return e
    }, Ct = function(e, n) {
        for (var i in n)
            i in e || (e[i] = n[i]);
        return e
    }, rg = function(e) {
        return function(n, i) {
            for (var s in i)
                s in n || s === "duration" && e || s === "ease" || (n[s] = i[s])
        }
    }, xi = function(e, n) {
        for (var i in n)
            e[i] = n[i];
        return e
    }, Fl = function t(e, n) {
        for (var i in n)
            i !== "__proto__" && i !== "constructor" && i !== "prototype" && (e[i] = rn(n[i]) ? t(e[i] || (e[i] = {}), n[i]) : n[i]);
        return e
    }, or = function(e, n) {
        var i = {}, s;
        for (s in e)
            s in n || (i[s] = e[s]);
        return i
    }, is = function(e) {
        var n = e.parent || Se
          , i = e.keyframes ? rg(nt(e.keyframes)) : Ct;
        if (ht(e.inherit))
            for (; n; )
                i(e, n.vars.defaults),
                n = n.parent || n._dp;
        return e
    }, og = function(e, n) {
        for (var i = e.length, s = i === n.length; s && i-- && e[i] === n[i]; )
            ;
        return i < 0
    }, Pf = function(e, n, i, s, r) {
        var o = e[s], a;
        if (r)
            for (a = n[r]; o && o[r] > a; )
                o = o._prev;
        return o ? (n._next = o._next,
        o._next = n) : (n._next = e[i],
        e[i] = n),
        n._next ? n._next._prev = n : e[s] = n,
        n._prev = o,
        n.parent = n._dp = e,
        n
    }, wr = function(e, n, i, s) {
        i === void 0 && (i = "_first"),
        s === void 0 && (s = "_last");
        var r = n._prev
          , o = n._next;
        r ? r._next = o : e[i] === n && (e[i] = o),
        o ? o._prev = r : e[s] === n && (e[s] = r),
        n._next = n._prev = n.parent = null
    }, In = function(e, n) {
        e.parent && (!n || e.parent.autoRemoveChildren) && e.parent.remove && e.parent.remove(e),
        e._act = 0
    }, ii = function(e, n) {
        if (e && (!n || n._end > e._dur || n._start < 0))
            for (var i = e; i; )
                i._dirty = 1,
                i = i.parent;
        return e
    }, ag = function(e) {
        for (var n = e.parent; n && n.parent; )
            n._dirty = 1,
            n.totalDuration(),
            n = n.parent;
        return e
    }, xo = function(e, n, i, s) {
        return e._startAt && (We ? e._startAt.revert(Vs) : e.vars.immediateRender && !e.vars.autoRevert || e._startAt.render(n, !0, s))
    }, lg = function t(e) {
        return !e || e._ts && t(e.parent)
    }, Gl = function(e) {
        return e._repeat ? Ii(e._tTime, e = e.duration() + e._rDelay) * e : 0
    }, Ii = function(e, n) {
        var i = Math.floor(e = Ge(e / n));
        return e && i === e ? i - 1 : i
    }, ar = function(e, n) {
        return (e - n._start) * n._ts + (n._ts >= 0 ? 0 : n._dirty ? n.totalDuration() : n._tDur)
    }, Sr = function(e) {
        return e._end = Ge(e._start + (e._tDur / Math.abs(e._ts || e._rts || tt) || 0))
    }, Or = function(e, n) {
        var i = e._dp;
        return i && i.smoothChildTiming && e._ts && (e._start = Ge(i._time - (e._ts > 0 ? n / e._ts : ((e._dirty ? e.totalDuration() : e._tDur) - n) / -e._ts)),
        Sr(e),
        i._dirty || ii(i, e)),
        e
    }, xf = function(e, n) {
        var i;
        if ((n._time || !n._dur && n._initted || n._start < e._time && (n._dur || !n.add)) && (i = ar(e.rawTime(), n),
        (!n._dur || Ps(0, n.totalDuration(), i) - n._tTime > tt) && n.render(i, !0)),
        ii(e, n)._dp && e._initted && e._time >= e._dur && e._ts) {
            if (e._dur < e.duration())
                for (i = e; i._dp; )
                    i.rawTime() >= 0 && i.totalTime(i._tTime),
                    i = i._dp;
            e._zTime = -1e-8
        }
    }, Jt = function(e, n, i, s) {
        return n.parent && In(n),
        n._start = Ge((mn(i) ? i : i || e !== Se ? Rt(e, i, n) : e._time) + n._delay),
        n._end = Ge(n._start + (n.totalDuration() / Math.abs(n.timeScale()) || 0)),
        Pf(e, n, "_first", "_last", e._sort ? "_start" : 0),
        Io(n) || (e._recent = n),
        s || xf(e, n),
        e._ts < 0 && Or(e, e._tTime),
        e
    }, If = function(e, n) {
        return (It.ScrollTrigger || Ta("scrollTrigger", n)) && It.ScrollTrigger.create(n, e)
    }, Cf = function(e, n, i, s, r) {
        if (Pa(e, n, r),
        !e._initted)
            return 1;
        if (!i && e._pt && !We && (e._dur && e.vars.lazy !== !1 || !e._dur && e.vars.lazy) && Tf !== wt.frame)
            return On.push(e),
            e._lazy = [r, s],
            1
    }, cg = function t(e) {
        var n = e.parent;
        return n && n._ts && n._initted && !n._lock && (n.rawTime() < 0 || t(n))
    }, Io = function(e) {
        var n = e.data;
        return n === "isFromStart" || n === "isStart"
    }, ug = function(e, n, i, s) {
        var r = e.ratio, o = n < 0 || !n && (!e._start && cg(e) && !(!e._initted && Io(e)) || (e._ts < 0 || e._dp._ts < 0) && !Io(e)) ? 0 : 1, a = e._rDelay, l = 0, c, u, f;
        if (a && e._repeat && (l = Ps(0, e._tDur, n),
        u = Ii(l, a),
        e._yoyo && u & 1 && (o = 1 - o),
        u !== Ii(e._tTime, a) && (r = 1 - o,
        e.vars.repeatRefresh && e._initted && e.invalidate())),
        o !== r || We || s || e._zTime === tt || !n && e._zTime) {
            if (!e._initted && Cf(e, n, s, i, l))
                return;
            for (f = e._zTime,
            e._zTime = n || (i ? tt : 0),
            i || (i = n && !f),
            e.ratio = o,
            e._from && (o = 1 - o),
            e._time = 0,
            e._tTime = l,
            c = e._pt; c; )
                c.r(o, c.d),
                c = c._next;
            n < 0 && xo(e, n, i, !0),
            e._onUpdate && !i && Ot(e, "onUpdate"),
            l && e._repeat && !i && e.parent && Ot(e, "onRepeat"),
            (n >= e._tDur || n < 0) && e.ratio === o && (o && In(e, 1),
            !i && !We && (Ot(e, o ? "onComplete" : "onReverseComplete", !0),
            e._prom && e._prom()))
        } else
            e._zTime || (e._zTime = n)
    }, fg = function(e, n, i) {
        var s;
        if (i > n)
            for (s = e._first; s && s._start <= i; ) {
                if (s.data === "isPause" && s._start > n)
                    return s;
                s = s._next
            }
        else
            for (s = e._last; s && s._start >= i; ) {
                if (s.data === "isPause" && s._start < n)
                    return s;
                s = s._prev
            }
    }, Ci = function(e, n, i, s) {
        var r = e._repeat
          , o = Ge(n) || 0
          , a = e._tTime / e._tDur;
        return a && !s && (e._time *= o / e._dur),
        e._dur = o,
        e._tDur = r ? r < 0 ? 1e10 : Ge(o * (r + 1) + e._rDelay * r) : o,
        a > 0 && !s && Or(e, e._tTime = e._tDur * a),
        e.parent && Sr(e),
        i || ii(e.parent, e),
        e
    }, Ul = function(e) {
        return e instanceof ut ? ii(e) : Ci(e, e._dur)
    }, dg = {
        _start: 0,
        endTime: hs,
        totalDuration: hs
    }, Rt = function t(e, n, i) {
        var s = e.labels, r = e._recent || dg, o = e.duration() >= tn ? r.endTime(!1) : e._dur, a, l, c;
        return He(n) && (isNaN(n) || n in s) ? (l = n.charAt(0),
        c = n.substr(-1) === "%",
        a = n.indexOf("="),
        l === "<" || l === ">" ? (a >= 0 && (n = n.replace(/=/, "")),
        (l === "<" ? r._start : r.endTime(r._repeat >= 0)) + (parseFloat(n.substr(1)) || 0) * (c ? (a < 0 ? r : i).totalDuration() / 100 : 1)) : a < 0 ? (n in s || (s[n] = o),
        s[n]) : (l = parseFloat(n.charAt(a - 1) + n.substr(a + 1)),
        c && i && (l = l / 100 * (nt(i) ? i[0] : i).totalDuration()),
        a > 1 ? t(e, n.substr(0, a - 1), i) + l : o + l)) : n == null ? o : +n
    }, ss = function(e, n, i) {
        var s = mn(n[1]), r = (s ? 2 : 1) + (e < 2 ? 0 : 1), o = n[r], a, l;
        if (s && (o.duration = n[1]),
        o.parent = i,
        e) {
            for (a = o,
            l = i; l && !("immediateRender"in a); )
                a = l.vars.defaults || {},
                l = ht(l.vars.inherit) && l.parent;
            o.immediateRender = ht(a.immediateRender),
            e < 2 ? o.runBackwards = 1 : o.startAt = n[r - 1]
        }
        return new Fe(n[0],o,n[r + 1])
    }, Mn = function(e, n) {
        return e || e === 0 ? n(e) : n
    }, Ps = function(e, n, i) {
        return i < e ? e : i > n ? n : i
    }, et = function(e, n) {
        return !He(e) || !(n = tg.exec(e)) ? "" : n[1]
    }, pg = function(e, n, i) {
        return Mn(i, function(s) {
            return Ps(e, n, s)
        })
    }, Co = [].slice, Rf = function(e, n) {
        return e && rn(e) && "length"in e && (!n && !e.length || e.length - 1 in e && rn(e[0])) && !e.nodeType && e !== Yt
    }, _g = function(e, n, i) {
        return i === void 0 && (i = []),
        e.forEach(function(s) {
            var r;
            return He(s) && !n || Rf(s, 1) ? (r = i).push.apply(r, Nt(s)) : i.push(s)
        }) || i
    }, Nt = function(e, n, i) {
        return Ee && !n && Ee.selector ? Ee.selector(e) : He(e) && !i && (Oo || !Ri()) ? Co.call((n || $a).querySelectorAll(e), 0) : nt(e) ? _g(e, i) : Rf(e) ? Co.call(e, 0) : e ? [e] : []
    }, Ro = function(e) {
        return e = Nt(e)[0] || _s("Invalid scope") || {},
        function(n) {
            var i = e.current || e.nativeElement || e;
            return Nt(n, i.querySelectorAll ? i : i === e ? _s("Invalid scope") || $a.createElement("div") : e)
        }
    }, Lf = function(e) {
        return e.sort(function() {
            return .5 - Math.random()
        })
    }, Nf = function(e) {
        if (Le(e))
            return e;
        var n = rn(e) ? e : {
            each: e
        }
          , i = si(n.ease)
          , s = n.from || 0
          , r = parseFloat(n.base) || 0
          , o = {}
          , a = s > 0 && s < 1
          , l = isNaN(s) || a
          , c = n.axis
          , u = s
          , f = s;
        return He(s) ? u = f = {
            center: .5,
            edges: .5,
            end: 1
        }[s] || 0 : !a && l && (u = s[0],
        f = s[1]),
        function(d, _, m) {
            var p = (m || n).length, b = o[p], E, S, v, y, A, T, O, C, P;
            if (!b) {
                if (P = n.grid === "auto" ? 0 : (n.grid || [1, tn])[1],
                !P) {
                    for (O = -1e8; O < (O = m[P++].getBoundingClientRect().left) && P < p; )
                        ;
                    P < p && P--
                }
                for (b = o[p] = [],
                E = l ? Math.min(P, p) * u - .5 : s % P,
                S = P === tn ? 0 : l ? p * f / P - .5 : s / P | 0,
                O = 0,
                C = tn,
                T = 0; T < p; T++)
                    v = T % P - E,
                    y = S - (T / P | 0),
                    b[T] = A = c ? Math.abs(c === "y" ? y : v) : mf(v * v + y * y),
                    A > O && (O = A),
                    A < C && (C = A);
                s === "random" && Lf(b),
                b.max = O - C,
                b.min = C,
                b.v = p = (parseFloat(n.amount) || parseFloat(n.each) * (P > p ? p - 1 : c ? c === "y" ? p / P : P : Math.max(P, p / P)) || 0) * (s === "edges" ? -1 : 1),
                b.b = p < 0 ? r - p : r,
                b.u = et(n.amount || n.each) || 0,
                i = i && p < 0 ? Hf(i) : i
            }
            return p = (b[d] - b.min) / b.max || 0,
            Ge(b.b + (i ? i(p) : p) * b.v) + b.u
        }
    }, Lo = function(e) {
        var n = Math.pow(10, ((e + "").split(".")[1] || "").length);
        return function(i) {
            var s = Ge(Math.round(parseFloat(i) / e) * e * n);
            return (s - s % 1) / n + (mn(i) ? 0 : et(i))
        }
    }, Df = function(e, n) {
        var i = nt(e), s, r;
        return !i && rn(e) && (s = i = e.radius || tn,
        e.values ? (e = Nt(e.values),
        (r = !mn(e[0])) && (s *= s)) : e = Lo(e.increment)),
        Mn(n, i ? Le(e) ? function(o) {
            return r = e(o),
            Math.abs(r - o) <= s ? r : o
        }
        : function(o) {
            for (var a = parseFloat(r ? o.x : o), l = parseFloat(r ? o.y : 0), c = tn, u = 0, f = e.length, d, _; f--; )
                r ? (d = e[f].x - a,
                _ = e[f].y - l,
                d = d * d + _ * _) : d = Math.abs(e[f] - a),
                d < c && (c = d,
                u = f);
            return u = !s || c <= s ? e[u] : o,
            r || u === o || mn(o) ? u : u + et(o)
        }
        : Lo(e))
    }, Mf = function(e, n, i, s) {
        return Mn(nt(e) ? !n : i === !0 ? !!(i = 0) : !s, function() {
            return nt(e) ? e[~~(Math.random() * e.length)] : (i = i || 1e-5) && (s = i < 1 ? Math.pow(10, (i + "").length - 2) : 1) && Math.floor(Math.round((e - i / 2 + Math.random() * (n - e + i * .99)) / i) * i * s) / s
        })
    }, hg = function() {
        for (var e = arguments.length, n = new Array(e), i = 0; i < e; i++)
            n[i] = arguments[i];
        return function(s) {
            return n.reduce(function(r, o) {
                return o(r)
            }, s)
        }
    }, mg = function(e, n) {
        return function(i) {
            return e(parseFloat(i)) + (n || et(i))
        }
    }, gg = function(e, n, i) {
        return Gf(e, n, 0, 1, i)
    }, Ff = function(e, n, i) {
        return Mn(i, function(s) {
            return e[~~n(s)]
        })
    }, bg = function t(e, n, i) {
        var s = n - e;
        return nt(e) ? Ff(e, t(0, e.length), n) : Mn(i, function(r) {
            return (s + (r - e) % s) % s + e
        })
    }, yg = function t(e, n, i) {
        var s = n - e
          , r = s * 2;
        return nt(e) ? Ff(e, t(0, e.length - 1), n) : Mn(i, function(o) {
            return o = (r + (o - e) % r) % r || 0,
            e + (o > s ? r - o : o)
        })
    }, ms = function(e) {
        for (var n = 0, i = "", s, r, o, a; ~(s = e.indexOf("random(", n)); )
            o = e.indexOf(")", s),
            a = e.charAt(s + 7) === "[",
            r = e.substr(s + 7, o - s - 7).match(a ? vf : So),
            i += e.substr(n, s - n) + Mf(a ? r : +r[0], a ? 0 : +r[1], +r[2] || 1e-5),
            n = o + 1;
        return i + e.substr(n, e.length - n)
    }, Gf = function(e, n, i, s, r) {
        var o = n - e
          , a = s - i;
        return Mn(r, function(l) {
            return i + ((l - e) / o * a || 0)
        })
    }, vg = function t(e, n, i, s) {
        var r = isNaN(e + n) ? 0 : function(_) {
            return (1 - _) * e + _ * n
        }
        ;
        if (!r) {
            var o = He(e), a = {}, l, c, u, f, d;
            if (i === !0 && (s = 1) && (i = null),
            o)
                e = {
                    p: e
                },
                n = {
                    p: n
                };
            else if (nt(e) && !nt(n)) {
                for (u = [],
                f = e.length,
                d = f - 2,
                c = 1; c < f; c++)
                    u.push(t(e[c - 1], e[c]));
                f--,
                r = function(m) {
                    m *= f;
                    var p = Math.min(d, ~~m);
                    return u[p](m - p)
                }
                ,
                i = n
            } else
                s || (e = xi(nt(e) ? [] : {}, e));
            if (!u) {
                for (l in n)
                    Oa.call(a, e, l, "get", n[l]);
                r = function(m) {
                    return Ca(m, a) || (o ? e.p : e)
                }
            }
        }
        return Mn(i, r)
    }, jl = function(e, n, i) {
        var s = e.labels, r = tn, o, a, l;
        for (o in s)
            a = s[o] - n,
            a < 0 == !!i && a && r > (a = Math.abs(a)) && (l = o,
            r = a);
        return l
    }, Ot = function(e, n, i) {
        var s = e.vars, r = s[n], o = Ee, a = e._ctx, l, c, u;
        if (r)
            return l = s[n + "Params"],
            c = s.callbackScope || e,
            i && On.length && rr(),
            a && (Ee = a),
            u = l ? r.apply(c, l) : r.call(c),
            Ee = o,
            u
    }, qi = function(e) {
        return In(e),
        e.scrollTrigger && e.scrollTrigger.kill(!!We),
        e.progress() < 1 && Ot(e, "onInterrupt"),
        e
    }, yi, Uf = [], jf = function(e) {
        if (e)
            if (e = !e.name && e.default || e,
            Aa() || e.headless) {
                var n = e.name
                  , i = Le(e)
                  , s = n && !i && e.init ? function() {
                    this._props = []
                }
                : e
                  , r = {
                    init: hs,
                    render: Ca,
                    add: Oa,
                    kill: Dg,
                    modifier: Ng,
                    rawVars: 0
                }
                  , o = {
                    targetTest: 0,
                    get: 0,
                    getSetter: Ia,
                    aliases: {},
                    register: 0
                };
                if (Ri(),
                e !== s) {
                    if (Et[n])
                        return;
                    Ct(s, Ct(or(e, r), o)),
                    xi(s.prototype, xi(r, or(e, o))),
                    Et[s.prop = n] = s,
                    e.targetTest && (Hs.push(s),
                    Ea[n] = 1),
                    n = (n === "css" ? "CSS" : n.charAt(0).toUpperCase() + n.substr(1)) + "Plugin"
                }
                $f(n, s),
                e.register && e.register(yt, s, gt)
            } else
                Uf.push(e)
    }, ve = 255, Wi = {
        aqua: [0, ve, ve],
        lime: [0, ve, 0],
        silver: [192, 192, 192],
        black: [0, 0, 0],
        maroon: [128, 0, 0],
        teal: [0, 128, 128],
        blue: [0, 0, ve],
        navy: [0, 0, 128],
        white: [ve, ve, ve],
        olive: [128, 128, 0],
        yellow: [ve, ve, 0],
        orange: [ve, 165, 0],
        gray: [128, 128, 128],
        purple: [128, 0, 128],
        green: [0, 128, 0],
        red: [ve, 0, 0],
        pink: [ve, 192, 203],
        cyan: [0, ve, ve],
        transparent: [ve, ve, ve, 0]
    }, Zr = function(e, n, i) {
        return e += e < 0 ? 1 : e > 1 ? -1 : 0,
        (e * 6 < 1 ? n + (i - n) * e * 6 : e < .5 ? i : e * 3 < 2 ? n + (i - n) * (2 / 3 - e) * 6 : n) * ve + .5 | 0
    }, Bf = function(e, n, i) {
        var s = e ? mn(e) ? [e >> 16, e >> 8 & ve, e & ve] : 0 : Wi.black, r, o, a, l, c, u, f, d, _, m;
        if (!s) {
            if (e.substr(-1) === "," && (e = e.substr(0, e.length - 1)),
            Wi[e])
                s = Wi[e];
            else if (e.charAt(0) === "#") {
                if (e.length < 6 && (r = e.charAt(1),
                o = e.charAt(2),
                a = e.charAt(3),
                e = "#" + r + r + o + o + a + a + (e.length === 5 ? e.charAt(4) + e.charAt(4) : "")),
                e.length === 9)
                    return s = parseInt(e.substr(1, 6), 16),
                    [s >> 16, s >> 8 & ve, s & ve, parseInt(e.substr(7), 16) / 255];
                e = parseInt(e.substr(1), 16),
                s = [e >> 16, e >> 8 & ve, e & ve]
            } else if (e.substr(0, 3) === "hsl") {
                if (s = m = e.match(So),
                !n)
                    l = +s[0] % 360 / 360,
                    c = +s[1] / 100,
                    u = +s[2] / 100,
                    o = u <= .5 ? u * (c + 1) : u + c - u * c,
                    r = u * 2 - o,
                    s.length > 3 && (s[3] *= 1),
                    s[0] = Zr(l + 1 / 3, r, o),
                    s[1] = Zr(l, r, o),
                    s[2] = Zr(l - 1 / 3, r, o);
                else if (~e.indexOf("="))
                    return s = e.match(bf),
                    i && s.length < 4 && (s[3] = 1),
                    s
            } else
                s = e.match(So) || Wi.transparent;
            s = s.map(Number)
        }
        return n && !m && (r = s[0] / ve,
        o = s[1] / ve,
        a = s[2] / ve,
        f = Math.max(r, o, a),
        d = Math.min(r, o, a),
        u = (f + d) / 2,
        f === d ? l = c = 0 : (_ = f - d,
        c = u > .5 ? _ / (2 - f - d) : _ / (f + d),
        l = f === r ? (o - a) / _ + (o < a ? 6 : 0) : f === o ? (a - r) / _ + 2 : (r - o) / _ + 4,
        l *= 60),
        s[0] = ~~(l + .5),
        s[1] = ~~(c * 100 + .5),
        s[2] = ~~(u * 100 + .5)),
        i && s.length < 4 && (s[3] = 1),
        s
    }, zf = function(e) {
        var n = []
          , i = []
          , s = -1;
        return e.split(Pn).forEach(function(r) {
            var o = r.match(bi) || [];
            n.push.apply(n, o),
            i.push(s += o.length + 1)
        }),
        n.c = i,
        n
    }, Bl = function(e, n, i) {
        var s = "", r = (e + s).match(Pn), o = n ? "hsla(" : "rgba(", a = 0, l, c, u, f;
        if (!r)
            return e;
        if (r = r.map(function(d) {
            return (d = Bf(d, n, 1)) && o + (n ? d[0] + "," + d[1] + "%," + d[2] + "%," + d[3] : d.join(",")) + ")"
        }),
        i && (u = zf(e),
        l = i.c,
        l.join(s) !== u.c.join(s)))
            for (c = e.replace(Pn, "1").split(bi),
            f = c.length - 1; a < f; a++)
                s += c[a] + (~l.indexOf(a) ? r.shift() || o + "0,0,0,0)" : (u.length ? u : r.length ? r : i).shift());
        if (!c)
            for (c = e.split(Pn),
            f = c.length - 1; a < f; a++)
                s += c[a] + r[a];
        return s + c[f]
    }, Pn = function() {
        var t = "(?:\\b(?:(?:rgb|rgba|hsl|hsla)\\(.+?\\))|\\B#(?:[0-9a-f]{3,4}){1,2}\\b", e;
        for (e in Wi)
            t += "|" + e + "\\b";
        return new RegExp(t + ")","gi")
    }(), kg = /hsl[a]?\(/, Vf = function(e) {
        var n = e.join(" "), i;
        if (Pn.lastIndex = 0,
        Pn.test(n))
            return i = kg.test(n),
            e[1] = Bl(e[1], i),
            e[0] = Bl(e[0], i, zf(e[1])),
            !0
    }, gs, wt = function() {
        var t = Date.now, e = 500, n = 33, i = t(), s = i, r = 1e3 / 240, o = r, a = [], l, c, u, f, d, _, m = function p(b) {
            var E = t() - s, S = b === !0, v, y, A, T;
            if ((E > e || E < 0) && (i += E - n),
            s += E,
            A = s - i,
            v = A - o,
            (v > 0 || S) && (T = ++f.frame,
            d = A - f.time * 1e3,
            f.time = A = A / 1e3,
            o += v + (v >= r ? 4 : r - v),
            y = 1),
            S || (l = c(p)),
            y)
                for (_ = 0; _ < a.length; _++)
                    a[_](A, d, T, b)
        };
        return f = {
            time: 0,
            frame: 0,
            tick: function() {
                m(!0)
            },
            deltaRatio: function(b) {
                return d / (1e3 / (b || 60))
            },
            wake: function() {
                kf && (!Oo && Aa() && (Yt = Oo = window,
                $a = Yt.document || {},
                It.gsap = yt,
                (Yt.gsapVersions || (Yt.gsapVersions = [])).push(yt.version),
                Af(sr || Yt.GreenSockGlobals || !Yt.gsap && Yt || {}),
                Uf.forEach(jf)),
                u = typeof requestAnimationFrame != "undefined" && requestAnimationFrame,
                l && f.sleep(),
                c = u || function(b) {
                    return setTimeout(b, o - f.time * 1e3 + 1 | 0)
                }
                ,
                gs = 1,
                m(2))
            },
            sleep: function() {
                (u ? cancelAnimationFrame : clearTimeout)(l),
                gs = 0,
                c = hs
            },
            lagSmoothing: function(b, E) {
                e = b || 1 / 0,
                n = Math.min(E || 33, e)
            },
            fps: function(b) {
                r = 1e3 / (b || 240),
                o = f.time * 1e3 + r
            },
            add: function(b, E, S) {
                var v = E ? function(y, A, T, O) {
                    b(y, A, T, O),
                    f.remove(v)
                }
                : b;
                return f.remove(b),
                a[S ? "unshift" : "push"](v),
                Ri(),
                v
            },
            remove: function(b, E) {
                ~(E = a.indexOf(b)) && a.splice(E, 1) && _ >= E && _--
            },
            _listeners: a
        },
        f
    }(), Ri = function() {
        return !gs && wt.wake()
    }, le = {}, Ag = /^[\d.\-M][\d.\-,\s]/, $g = /["']/g, Tg = function(e) {
        for (var n = {}, i = e.substr(1, e.length - 3).split(":"), s = i[0], r = 1, o = i.length, a, l, c; r < o; r++)
            l = i[r],
            a = r !== o - 1 ? l.lastIndexOf(",") : l.length,
            c = l.substr(0, a),
            n[s] = isNaN(c) ? c.replace($g, "").trim() : +c,
            s = l.substr(a + 1).trim();
        return n
    }, Eg = function(e) {
        var n = e.indexOf("(") + 1
          , i = e.indexOf(")")
          , s = e.indexOf("(", n);
        return e.substring(n, ~s && s < i ? e.indexOf(")", i + 1) : i)
    }, wg = function(e) {
        var n = (e + "").split("(")
          , i = le[n[0]];
        return i && n.length > 1 && i.config ? i.config.apply(null, ~e.indexOf("{") ? [Tg(n[1])] : Eg(e).split(",").map(Sf)) : le._CE && Ag.test(e) ? le._CE("", e) : i
    }, Hf = function(e) {
        return function(n) {
            return 1 - e(1 - n)
        }
    }, Kf = function t(e, n) {
        for (var i = e._first, s; i; )
            i instanceof ut ? t(i, n) : i.vars.yoyoEase && (!i._yoyo || !i._repeat) && i._yoyo !== n && (i.timeline ? t(i.timeline, n) : (s = i._ease,
            i._ease = i._yEase,
            i._yEase = s,
            i._yoyo = n)),
            i = i._next
    }, si = function(e, n) {
        return e && (Le(e) ? e : le[e] || wg(e)) || n
    }, fi = function(e, n, i, s) {
        i === void 0 && (i = function(l) {
            return 1 - n(1 - l)
        }
        ),
        s === void 0 && (s = function(l) {
            return l < .5 ? n(l * 2) / 2 : 1 - n((1 - l) * 2) / 2
        }
        );
        var r = {
            easeIn: n,
            easeOut: i,
            easeInOut: s
        }, o;
        return mt(e, function(a) {
            le[a] = It[a] = r,
            le[o = a.toLowerCase()] = i;
            for (var l in r)
                le[o + (l === "easeIn" ? ".in" : l === "easeOut" ? ".out" : ".inOut")] = le[a + "." + l] = r[l]
        }),
        r
    }, qf = function(e) {
        return function(n) {
            return n < .5 ? (1 - e(1 - n * 2)) / 2 : .5 + e((n - .5) * 2) / 2
        }
    }, eo = function t(e, n, i) {
        var s = n >= 1 ? n : 1
          , r = (i || (e ? .3 : .45)) / (n < 1 ? n : 1)
          , o = r / wo * (Math.asin(1 / s) || 0)
          , a = function(u) {
            return u === 1 ? 1 : s * Math.pow(2, -10 * u) * eg((u - o) * r) + 1
        }
          , l = e === "out" ? a : e === "in" ? function(c) {
            return 1 - a(1 - c)
        }
        : qf(a);
        return r = wo / r,
        l.config = function(c, u) {
            return t(e, c, u)
        }
        ,
        l
    }, to = function t(e, n) {
        n === void 0 && (n = 1.70158);
        var i = function(o) {
            return o ? --o * o * ((n + 1) * o + n) + 1 : 0
        }
          , s = e === "out" ? i : e === "in" ? function(r) {
            return 1 - i(1 - r)
        }
        : qf(i);
        return s.config = function(r) {
            return t(e, r)
        }
        ,
        s
    };
    mt("Linear,Quad,Cubic,Quart,Quint,Strong", function(t, e) {
        var n = e < 5 ? e + 1 : e;
        fi(t + ",Power" + (n - 1), e ? function(i) {
            return Math.pow(i, n)
        }
        : function(i) {
            return i
        }
        , function(i) {
            return 1 - Math.pow(1 - i, n)
        }, function(i) {
            return i < .5 ? Math.pow(i * 2, n) / 2 : 1 - Math.pow((1 - i) * 2, n) / 2
        })
    });
    le.Linear.easeNone = le.none = le.Linear.easeIn;
    fi("Elastic", eo("in"), eo("out"), eo());
    (function(t, e) {
        var n = 1 / e
          , i = 2 * n
          , s = 2.5 * n
          , r = function(a) {
            return a < n ? t * a * a : a < i ? t * Math.pow(a - 1.5 / e, 2) + .75 : a < s ? t * (a -= 2.25 / e) * a + .9375 : t * Math.pow(a - 2.625 / e, 2) + .984375
        };
        fi("Bounce", function(o) {
            return 1 - r(1 - o)
        }, r)
    }
    )(7.5625, 2.75);
    fi("Expo", function(t) {
        return Math.pow(2, 10 * (t - 1)) * t + t * t * t * t * t * t * (1 - t)
    });
    fi("Circ", function(t) {
        return -(mf(1 - t * t) - 1)
    });
    fi("Sine", function(t) {
        return t === 1 ? 1 : -Zm(t * Jm) + 1
    });
    fi("Back", to("in"), to("out"), to());
    le.SteppedEase = le.steps = It.SteppedEase = {
        config: function(e, n) {
            e === void 0 && (e = 1);
            var i = 1 / e
              , s = e + (n ? 0 : 1)
              , r = n ? 1 : 0
              , o = 1 - tt;
            return function(a) {
                return ((s * Ps(0, o, a) | 0) + r) * i
            }
        }
    };
    Pi.ease = le["quad.out"];
    mt("onComplete,onUpdate,onStart,onRepeat,onReverseComplete,onInterrupt", function(t) {
        return wa += t + "," + t + "Params,"
    });
    var Wf = function(e, n) {
        this.id = Qm++,
        e._gsap = this,
        this.target = e,
        this.harness = n,
        this.get = n ? n.get : Ef,
        this.set = n ? n.getSetter : Ia
    }
      , bs = function() {
        function t(n) {
            this.vars = n,
            this._delay = +n.delay || 0,
            (this._repeat = n.repeat === 1 / 0 ? -2 : n.repeat || 0) && (this._rDelay = n.repeatDelay || 0,
            this._yoyo = !!n.yoyo || !!n.yoyoEase),
            this._ts = 1,
            Ci(this, +n.duration, 1, 1),
            this.data = n.data,
            Ee && (this._ctx = Ee,
            Ee.data.push(this)),
            gs || wt.wake()
        }
        var e = t.prototype;
        return e.delay = function(i) {
            return i || i === 0 ? (this.parent && this.parent.smoothChildTiming && this.startTime(this._start + i - this._delay),
            this._delay = i,
            this) : this._delay
        }
        ,
        e.duration = function(i) {
            return arguments.length ? this.totalDuration(this._repeat > 0 ? i + (i + this._rDelay) * this._repeat : i) : this.totalDuration() && this._dur
        }
        ,
        e.totalDuration = function(i) {
            return arguments.length ? (this._dirty = 0,
            Ci(this, this._repeat < 0 ? i : (i - this._repeat * this._rDelay) / (this._repeat + 1))) : this._tDur
        }
        ,
        e.totalTime = function(i, s) {
            if (Ri(),
            !arguments.length)
                return this._tTime;
            var r = this._dp;
            if (r && r.smoothChildTiming && this._ts) {
                for (Or(this, i),
                !r._dp || r.parent || xf(r, this); r && r.parent; )
                    r.parent._time !== r._start + (r._ts >= 0 ? r._tTime / r._ts : (r.totalDuration() - r._tTime) / -r._ts) && r.totalTime(r._tTime, !0),
                    r = r.parent;
                !this.parent && this._dp.autoRemoveChildren && (this._ts > 0 && i < this._tDur || this._ts < 0 && i > 0 || !this._tDur && !i) && Jt(this._dp, this, this._start - this._delay)
            }
            return (this._tTime !== i || !this._dur && !s || this._initted && Math.abs(this._zTime) === tt || !i && !this._initted && (this.add || this._ptLookup)) && (this._ts || (this._pTime = i),
            wf(this, i, s)),
            this
        }
        ,
        e.time = function(i, s) {
            return arguments.length ? this.totalTime(Math.min(this.totalDuration(), i + Gl(this)) % (this._dur + this._rDelay) || (i ? this._dur : 0), s) : this._time
        }
        ,
        e.totalProgress = function(i, s) {
            return arguments.length ? this.totalTime(this.totalDuration() * i, s) : this.totalDuration() ? Math.min(1, this._tTime / this._tDur) : this.rawTime() >= 0 && this._initted ? 1 : 0
        }
        ,
        e.progress = function(i, s) {
            return arguments.length ? this.totalTime(this.duration() * (this._yoyo && !(this.iteration() & 1) ? 1 - i : i) + Gl(this), s) : this.duration() ? Math.min(1, this._time / this._dur) : this.rawTime() > 0 ? 1 : 0
        }
        ,
        e.iteration = function(i, s) {
            var r = this.duration() + this._rDelay;
            return arguments.length ? this.totalTime(this._time + (i - 1) * r, s) : this._repeat ? Ii(this._tTime, r) + 1 : 1
        }
        ,
        e.timeScale = function(i, s) {
            if (!arguments.length)
                return this._rts === -1e-8 ? 0 : this._rts;
            if (this._rts === i)
                return this;
            var r = this.parent && this._ts ? ar(this.parent._time, this) : this._tTime;
            return this._rts = +i || 0,
            this._ts = this._ps || i === -1e-8 ? 0 : this._rts,
            this.totalTime(Ps(-Math.abs(this._delay), this._tDur, r), s !== !1),
            Sr(this),
            ag(this)
        }
        ,
        e.paused = function(i) {
            return arguments.length ? (this._ps !== i && (this._ps = i,
            i ? (this._pTime = this._tTime || Math.max(-this._delay, this.rawTime()),
            this._ts = this._act = 0) : (Ri(),
            this._ts = this._rts,
            this.totalTime(this.parent && !this.parent.smoothChildTiming ? this.rawTime() : this._tTime || this._pTime, this.progress() === 1 && Math.abs(this._zTime) !== tt && (this._tTime -= tt)))),
            this) : this._ps
        }
        ,
        e.startTime = function(i) {
            if (arguments.length) {
                this._start = i;
                var s = this.parent || this._dp;
                return s && (s._sort || !this.parent) && Jt(s, this, i - this._delay),
                this
            }
            return this._start
        }
        ,
        e.endTime = function(i) {
            return this._start + (ht(i) ? this.totalDuration() : this.duration()) / Math.abs(this._ts || 1)
        }
        ,
        e.rawTime = function(i) {
            var s = this.parent || this._dp;
            return s ? i && (!this._ts || this._repeat && this._time && this.totalProgress() < 1) ? this._tTime % (this._dur + this._rDelay) : this._ts ? ar(s.rawTime(i), this) : this._tTime : this._tTime
        }
        ,
        e.revert = function(i) {
            i === void 0 && (i = ig);
            var s = We;
            return We = i,
            (this._initted || this._startAt) && (this.timeline && this.timeline.revert(i),
            this.totalTime(-.01, i.suppressEvents)),
            this.data !== "nested" && i.kill !== !1 && this.kill(),
            We = s,
            this
        }
        ,
        e.globalTime = function(i) {
            for (var s = this, r = arguments.length ? i : s.rawTime(); s; )
                r = s._start + r / (Math.abs(s._ts) || 1),
                s = s._dp;
            return !this.parent && this._sat ? this._sat.globalTime(i) : r
        }
        ,
        e.repeat = function(i) {
            return arguments.length ? (this._repeat = i === 1 / 0 ? -2 : i,
            Ul(this)) : this._repeat === -2 ? 1 / 0 : this._repeat
        }
        ,
        e.repeatDelay = function(i) {
            if (arguments.length) {
                var s = this._time;
                return this._rDelay = i,
                Ul(this),
                s ? this.time(s) : this
            }
            return this._rDelay
        }
        ,
        e.yoyo = function(i) {
            return arguments.length ? (this._yoyo = i,
            this) : this._yoyo
        }
        ,
        e.seek = function(i, s) {
            return this.totalTime(Rt(this, i), ht(s))
        }
        ,
        e.restart = function(i, s) {
            return this.play().totalTime(i ? -this._delay : 0, ht(s)),
            this._dur || (this._zTime = -1e-8),
            this
        }
        ,
        e.play = function(i, s) {
            return i != null && this.seek(i, s),
            this.reversed(!1).paused(!1)
        }
        ,
        e.reverse = function(i, s) {
            return i != null && this.seek(i || this.totalDuration(), s),
            this.reversed(!0).paused(!1)
        }
        ,
        e.pause = function(i, s) {
            return i != null && this.seek(i, s),
            this.paused(!0)
        }
        ,
        e.resume = function() {
            return this.paused(!1)
        }
        ,
        e.reversed = function(i) {
            return arguments.length ? (!!i !== this.reversed() && this.timeScale(-this._rts || (i ? -1e-8 : 0)),
            this) : this._rts < 0
        }
        ,
        e.invalidate = function() {
            return this._initted = this._act = 0,
            this._zTime = -1e-8,
            this
        }
        ,
        e.isActive = function() {
            var i = this.parent || this._dp, s = this._start, r;
            return !!(!i || this._ts && this._initted && i.isActive() && (r = i.rawTime(!0)) >= s && r < this.endTime(!0) - tt)
        }
        ,
        e.eventCallback = function(i, s, r) {
            var o = this.vars;
            return arguments.length > 1 ? (s ? (o[i] = s,
            r && (o[i + "Params"] = r),
            i === "onUpdate" && (this._onUpdate = s)) : delete o[i],
            this) : o[i]
        }
        ,
        e.then = function(i) {
            var s = this;
            return new Promise(function(r) {
                var o = Le(i) ? i : Of
                  , a = function() {
                    var c = s.then;
                    s.then = null,
                    Le(o) && (o = o(s)) && (o.then || o === s) && (s.then = c),
                    r(o),
                    s.then = c
                };
                s._initted && s.totalProgress() === 1 && s._ts >= 0 || !s._tTime && s._ts < 0 ? a() : s._prom = a
            }
            )
        }
        ,
        e.kill = function() {
            qi(this)
        }
        ,
        t
    }();
    Ct(bs.prototype, {
        _time: 0,
        _start: 0,
        _end: 0,
        _tTime: 0,
        _tDur: 0,
        _dirty: 0,
        _repeat: 0,
        _yoyo: !1,
        parent: null,
        _initted: !1,
        _rDelay: 0,
        _ts: 1,
        _dp: 0,
        ratio: 0,
        _zTime: -1e-8,
        _prom: 0,
        _ps: !1,
        _rts: 1
    });
    var ut = function(t) {
        hf(e, t);
        function e(i, s) {
            var r;
            return i === void 0 && (i = {}),
            r = t.call(this, i) || this,
            r.labels = {},
            r.smoothChildTiming = !!i.smoothChildTiming,
            r.autoRemoveChildren = !!i.autoRemoveChildren,
            r._sort = ht(i.sortChildren),
            Se && Jt(i.parent || Se, dn(r), s),
            i.reversed && r.reverse(),
            i.paused && r.paused(!0),
            i.scrollTrigger && If(dn(r), i.scrollTrigger),
            r
        }
        var n = e.prototype;
        return n.to = function(s, r, o) {
            return ss(0, arguments, this),
            this
        }
        ,
        n.from = function(s, r, o) {
            return ss(1, arguments, this),
            this
        }
        ,
        n.fromTo = function(s, r, o, a) {
            return ss(2, arguments, this),
            this
        }
        ,
        n.set = function(s, r, o) {
            return r.duration = 0,
            r.parent = this,
            is(r).repeatDelay || (r.repeat = 0),
            r.immediateRender = !!r.immediateRender,
            new Fe(s,r,Rt(this, o),1),
            this
        }
        ,
        n.call = function(s, r, o) {
            return Jt(this, Fe.delayedCall(0, s, r), o)
        }
        ,
        n.staggerTo = function(s, r, o, a, l, c, u) {
            return o.duration = r,
            o.stagger = o.stagger || a,
            o.onComplete = c,
            o.onCompleteParams = u,
            o.parent = this,
            new Fe(s,o,Rt(this, l)),
            this
        }
        ,
        n.staggerFrom = function(s, r, o, a, l, c, u) {
            return o.runBackwards = 1,
            is(o).immediateRender = ht(o.immediateRender),
            this.staggerTo(s, r, o, a, l, c, u)
        }
        ,
        n.staggerFromTo = function(s, r, o, a, l, c, u, f) {
            return a.startAt = o,
            is(a).immediateRender = ht(a.immediateRender),
            this.staggerTo(s, r, a, l, c, u, f)
        }
        ,
        n.render = function(s, r, o) {
            var a = this._time, l = this._dirty ? this.totalDuration() : this._tDur, c = this._dur, u = s <= 0 ? 0 : Ge(s), f = this._zTime < 0 != s < 0 && (this._initted || !c), d, _, m, p, b, E, S, v, y, A, T, O;
            if (this !== Se && u > l && s >= 0 && (u = l),
            u !== this._tTime || o || f) {
                if (a !== this._time && c && (u += this._time - a,
                s += this._time - a),
                d = u,
                y = this._start,
                v = this._ts,
                E = !v,
                f && (c || (a = this._zTime),
                (s || !r) && (this._zTime = s)),
                this._repeat) {
                    if (T = this._yoyo,
                    b = c + this._rDelay,
                    this._repeat < -1 && s < 0)
                        return this.totalTime(b * 100 + s, r, o);
                    if (d = Ge(u % b),
                    u === l ? (p = this._repeat,
                    d = c) : (A = Ge(u / b),
                    p = ~~A,
                    p && p === A && (d = c,
                    p--),
                    d > c && (d = c)),
                    A = Ii(this._tTime, b),
                    !a && this._tTime && A !== p && this._tTime - A * b - this._dur <= 0 && (A = p),
                    T && p & 1 && (d = c - d,
                    O = 1),
                    p !== A && !this._lock) {
                        var C = T && A & 1
                          , P = C === (T && p & 1);
                        if (p < A && (C = !C),
                        a = C ? 0 : u % c ? c : u,
                        this._lock = 1,
                        this.render(a || (O ? 0 : Ge(p * b)), r, !c)._lock = 0,
                        this._tTime = u,
                        !r && this.parent && Ot(this, "onRepeat"),
                        this.vars.repeatRefresh && !O && (this.invalidate()._lock = 1),
                        a && a !== this._time || E !== !this._ts || this.vars.onRepeat && !this.parent && !this._act)
                            return this;
                        if (c = this._dur,
                        l = this._tDur,
                        P && (this._lock = 2,
                        a = C ? c : -1e-4,
                        this.render(a, !0),
                        this.vars.repeatRefresh && !O && this.invalidate()),
                        this._lock = 0,
                        !this._ts && !E)
                            return this;
                        Kf(this, O)
                    }
                }
                if (this._hasPause && !this._forcing && this._lock < 2 && (S = fg(this, Ge(a), Ge(d)),
                S && (u -= d - (d = S._start))),
                this._tTime = u,
                this._time = d,
                this._act = !v,
                this._initted || (this._onUpdate = this.vars.onUpdate,
                this._initted = 1,
                this._zTime = s,
                a = 0),
                !a && d && !r && !p && (Ot(this, "onStart"),
                this._tTime !== u))
                    return this;
                if (d >= a && s >= 0)
                    for (_ = this._first; _; ) {
                        if (m = _._next,
                        (_._act || d >= _._start) && _._ts && S !== _) {
                            if (_.parent !== this)
                                return this.render(s, r, o);
                            if (_.render(_._ts > 0 ? (d - _._start) * _._ts : (_._dirty ? _.totalDuration() : _._tDur) + (d - _._start) * _._ts, r, o),
                            d !== this._time || !this._ts && !E) {
                                S = 0,
                                m && (u += this._zTime = -1e-8);
                                break
                            }
                        }
                        _ = m
                    }
                else {
                    _ = this._last;
                    for (var W = s < 0 ? s : d; _; ) {
                        if (m = _._prev,
                        (_._act || W <= _._end) && _._ts && S !== _) {
                            if (_.parent !== this)
                                return this.render(s, r, o);
                            if (_.render(_._ts > 0 ? (W - _._start) * _._ts : (_._dirty ? _.totalDuration() : _._tDur) + (W - _._start) * _._ts, r, o || We && (_._initted || _._startAt)),
                            d !== this._time || !this._ts && !E) {
                                S = 0,
                                m && (u += this._zTime = W ? -1e-8 : tt);
                                break
                            }
                        }
                        _ = m
                    }
                }
                if (S && !r && (this.pause(),
                S.render(d >= a ? 0 : -1e-8)._zTime = d >= a ? 1 : -1,
                this._ts))
                    return this._start = y,
                    Sr(this),
                    this.render(s, r, o);
                this._onUpdate && !r && Ot(this, "onUpdate", !0),
                (u === l && this._tTime >= this.totalDuration() || !u && a) && (y === this._start || Math.abs(v) !== Math.abs(this._ts)) && (this._lock || ((s || !c) && (u === l && this._ts > 0 || !u && this._ts < 0) && In(this, 1),
                !r && !(s < 0 && !a) && (u || a || !l) && (Ot(this, u === l && s >= 0 ? "onComplete" : "onReverseComplete", !0),
                this._prom && !(u < l && this.timeScale() > 0) && this._prom())))
            }
            return this
        }
        ,
        n.add = function(s, r) {
            var o = this;
            if (mn(r) || (r = Rt(this, r, s)),
            !(s instanceof bs)) {
                if (nt(s))
                    return s.forEach(function(a) {
                        return o.add(a, r)
                    }),
                    this;
                if (He(s))
                    return this.addLabel(s, r);
                if (Le(s))
                    s = Fe.delayedCall(0, s);
                else
                    return this
            }
            return this !== s ? Jt(this, s, r) : this
        }
        ,
        n.getChildren = function(s, r, o, a) {
            s === void 0 && (s = !0),
            r === void 0 && (r = !0),
            o === void 0 && (o = !0),
            a === void 0 && (a = -1e8);
            for (var l = [], c = this._first; c; )
                c._start >= a && (c instanceof Fe ? r && l.push(c) : (o && l.push(c),
                s && l.push.apply(l, c.getChildren(!0, r, o)))),
                c = c._next;
            return l
        }
        ,
        n.getById = function(s) {
            for (var r = this.getChildren(1, 1, 1), o = r.length; o--; )
                if (r[o].vars.id === s)
                    return r[o]
        }
        ,
        n.remove = function(s) {
            return He(s) ? this.removeLabel(s) : Le(s) ? this.killTweensOf(s) : (s.parent === this && wr(this, s),
            s === this._recent && (this._recent = this._last),
            ii(this))
        }
        ,
        n.totalTime = function(s, r) {
            return arguments.length ? (this._forcing = 1,
            !this._dp && this._ts && (this._start = Ge(wt.time - (this._ts > 0 ? s / this._ts : (this.totalDuration() - s) / -this._ts))),
            t.prototype.totalTime.call(this, s, r),
            this._forcing = 0,
            this) : this._tTime
        }
        ,
        n.addLabel = function(s, r) {
            return this.labels[s] = Rt(this, r),
            this
        }
        ,
        n.removeLabel = function(s) {
            return delete this.labels[s],
            this
        }
        ,
        n.addPause = function(s, r, o) {
            var a = Fe.delayedCall(0, r || hs, o);
            return a.data = "isPause",
            this._hasPause = 1,
            Jt(this, a, Rt(this, s))
        }
        ,
        n.removePause = function(s) {
            var r = this._first;
            for (s = Rt(this, s); r; )
                r._start === s && r.data === "isPause" && In(r),
                r = r._next
        }
        ,
        n.killTweensOf = function(s, r, o) {
            for (var a = this.getTweensOf(s, o), l = a.length; l--; )
                $n !== a[l] && a[l].kill(s, r);
            return this
        }
        ,
        n.getTweensOf = function(s, r) {
            for (var o = [], a = Nt(s), l = this._first, c = mn(r), u; l; )
                l instanceof Fe ? sg(l._targets, a) && (c ? (!$n || l._initted && l._ts) && l.globalTime(0) <= r && l.globalTime(l.totalDuration()) > r : !r || l.isActive()) && o.push(l) : (u = l.getTweensOf(a, r)).length && o.push.apply(o, u),
                l = l._next;
            return o
        }
        ,
        n.tweenTo = function(s, r) {
            r = r || {};
            var o = this, a = Rt(o, s), l = r, c = l.startAt, u = l.onStart, f = l.onStartParams, d = l.immediateRender, _, m = Fe.to(o, Ct({
                ease: r.ease || "none",
                lazy: !1,
                immediateRender: !1,
                time: a,
                overwrite: "auto",
                duration: r.duration || Math.abs((a - (c && "time"in c ? c.time : o._time)) / o.timeScale()) || tt,
                onStart: function() {
                    if (o.pause(),
                    !_) {
                        var b = r.duration || Math.abs((a - (c && "time"in c ? c.time : o._time)) / o.timeScale());
                        m._dur !== b && Ci(m, b, 0, 1).render(m._time, !0, !0),
                        _ = 1
                    }
                    u && u.apply(m, f || [])
                }
            }, r));
            return d ? m.render(0) : m
        }
        ,
        n.tweenFromTo = function(s, r, o) {
            return this.tweenTo(r, Ct({
                startAt: {
                    time: Rt(this, s)
                }
            }, o))
        }
        ,
        n.recent = function() {
            return this._recent
        }
        ,
        n.nextLabel = function(s) {
            return s === void 0 && (s = this._time),
            jl(this, Rt(this, s))
        }
        ,
        n.previousLabel = function(s) {
            return s === void 0 && (s = this._time),
            jl(this, Rt(this, s), 1)
        }
        ,
        n.currentLabel = function(s) {
            return arguments.length ? this.seek(s, !0) : this.previousLabel(this._time + tt)
        }
        ,
        n.shiftChildren = function(s, r, o) {
            o === void 0 && (o = 0);
            for (var a = this._first, l = this.labels, c; a; )
                a._start >= o && (a._start += s,
                a._end += s),
                a = a._next;
            if (r)
                for (c in l)
                    l[c] >= o && (l[c] += s);
            return ii(this)
        }
        ,
        n.invalidate = function(s) {
            var r = this._first;
            for (this._lock = 0; r; )
                r.invalidate(s),
                r = r._next;
            return t.prototype.invalidate.call(this, s)
        }
        ,
        n.clear = function(s) {
            s === void 0 && (s = !0);
            for (var r = this._first, o; r; )
                o = r._next,
                this.remove(r),
                r = o;
            return this._dp && (this._time = this._tTime = this._pTime = 0),
            s && (this.labels = {}),
            ii(this)
        }
        ,
        n.totalDuration = function(s) {
            var r = 0, o = this, a = o._last, l = tn, c, u, f;
            if (arguments.length)
                return o.timeScale((o._repeat < 0 ? o.duration() : o.totalDuration()) / (o.reversed() ? -s : s));
            if (o._dirty) {
                for (f = o.parent; a; )
                    c = a._prev,
                    a._dirty && a.totalDuration(),
                    u = a._start,
                    u > l && o._sort && a._ts && !o._lock ? (o._lock = 1,
                    Jt(o, a, u - a._delay, 1)._lock = 0) : l = u,
                    u < 0 && a._ts && (r -= u,
                    (!f && !o._dp || f && f.smoothChildTiming) && (o._start += u / o._ts,
                    o._time -= u,
                    o._tTime -= u),
                    o.shiftChildren(-u, !1, -1 / 0),
                    l = 0),
                    a._end > r && a._ts && (r = a._end),
                    a = c;
                Ci(o, o === Se && o._time > r ? o._time : r, 1, 1),
                o._dirty = 0
            }
            return o._tDur
        }
        ,
        e.updateRoot = function(s) {
            if (Se._ts && (wf(Se, ar(s, Se)),
            Tf = wt.frame),
            wt.frame >= Ml) {
                Ml += xt.autoSleep || 120;
                var r = Se._first;
                if ((!r || !r._ts) && xt.autoSleep && wt._listeners.length < 2) {
                    for (; r && !r._ts; )
                        r = r._next;
                    r || wt.sleep()
                }
            }
        }
        ,
        e
    }(bs);
    Ct(ut.prototype, {
        _lock: 0,
        _hasPause: 0,
        _forcing: 0
    });
    var Sg = function(e, n, i, s, r, o, a) {
        var l = new gt(this._pt,e,n,0,1,ed,null,r), c = 0, u = 0, f, d, _, m, p, b, E, S;
        for (l.b = i,
        l.e = s,
        i += "",
        s += "",
        (E = ~s.indexOf("random(")) && (s = ms(s)),
        o && (S = [i, s],
        o(S, e, n),
        i = S[0],
        s = S[1]),
        d = i.match(Jr) || []; f = Jr.exec(s); )
            m = f[0],
            p = s.substring(c, f.index),
            _ ? _ = (_ + 1) % 5 : p.substr(-5) === "rgba(" && (_ = 1),
            m !== d[u++] && (b = parseFloat(d[u - 1]) || 0,
            l._pt = {
                _next: l._pt,
                p: p || u === 1 ? p : ",",
                s: b,
                c: m.charAt(1) === "=" ? Ei(b, m) - b : parseFloat(m) - b,
                m: _ && _ < 4 ? Math.round : 0
            },
            c = Jr.lastIndex);
        return l.c = c < s.length ? s.substring(c, s.length) : "",
        l.fp = a,
        (yf.test(s) || E) && (l.e = 0),
        this._pt = l,
        l
    }, Oa = function(e, n, i, s, r, o, a, l, c, u) {
        Le(s) && (s = s(r || 0, e, o));
        var f = e[n], d = i !== "get" ? i : Le(f) ? c ? e[n.indexOf("set") || !Le(e["get" + n.substr(3)]) ? n : "get" + n.substr(3)](c) : e[n]() : f, _ = Le(f) ? c ? Cg : Qf : xa, m;
        if (He(s) && (~s.indexOf("random(") && (s = ms(s)),
        s.charAt(1) === "=" && (m = Ei(d, s) + (et(d) || 0),
        (m || m === 0) && (s = m))),
        !u || d !== s || No)
            return !isNaN(d * s) && s !== "" ? (m = new gt(this._pt,e,n,+d || 0,s - (d || 0),typeof f == "boolean" ? Lg : Zf,0,_),
            c && (m.fp = c),
            a && m.modifier(a, this, e),
            this._pt = m) : (!f && !(n in e) && Ta(n, s),
            Sg.call(this, e, n, d, s, _, l || xt.stringFilter, c))
    }, Og = function(e, n, i, s, r) {
        if (Le(e) && (e = rs(e, r, n, i, s)),
        !rn(e) || e.style && e.nodeType || nt(e) || gf(e))
            return He(e) ? rs(e, r, n, i, s) : e;
        var o = {}, a;
        for (a in e)
            o[a] = rs(e[a], r, n, i, s);
        return o
    }, Yf = function(e, n, i, s, r, o) {
        var a, l, c, u;
        if (Et[e] && (a = new Et[e]).init(r, a.rawVars ? n[e] : Og(n[e], s, r, o, i), i, s, o) !== !1 && (i._pt = l = new gt(i._pt,r,e,0,1,a.render,a,0,a.priority),
        i !== yi))
            for (c = i._ptLookup[i._targets.indexOf(r)],
            u = a._props.length; u--; )
                c[a._props[u]] = l;
        return a
    }, $n, No, Pa = function t(e, n, i) {
        var s = e.vars, r = s.ease, o = s.startAt, a = s.immediateRender, l = s.lazy, c = s.onUpdate, u = s.runBackwards, f = s.yoyoEase, d = s.keyframes, _ = s.autoRevert, m = e._dur, p = e._startAt, b = e._targets, E = e.parent, S = E && E.data === "nested" ? E.vars.targets : b, v = e._overwrite === "auto" && !va, y = e.timeline, A, T, O, C, P, W, X, q, Q, _e, ue, Z, J;
        if (y && (!d || !r) && (r = "none"),
        e._ease = si(r, Pi.ease),
        e._yEase = f ? Hf(si(f === !0 ? r : f, Pi.ease)) : 0,
        f && e._yoyo && !e._repeat && (f = e._yEase,
        e._yEase = e._ease,
        e._ease = f),
        e._from = !y && !!s.runBackwards,
        !y || d && !s.stagger) {
            if (q = b[0] ? ni(b[0]).harness : 0,
            Z = q && s[q.prop],
            A = or(s, Ea),
            p && (p._zTime < 0 && p.progress(1),
            n < 0 && u && a && !_ ? p.render(-1, !0) : p.revert(u && m ? Vs : ng),
            p._lazy = 0),
            o) {
                if (In(e._startAt = Fe.set(b, Ct({
                    data: "isStart",
                    overwrite: !1,
                    parent: E,
                    immediateRender: !0,
                    lazy: !p && ht(l),
                    startAt: null,
                    delay: 0,
                    onUpdate: c && function() {
                        return Ot(e, "onUpdate")
                    }
                    ,
                    stagger: 0
                }, o))),
                e._startAt._dp = 0,
                e._startAt._sat = e,
                n < 0 && (We || !a && !_) && e._startAt.revert(Vs),
                a && m && n <= 0 && i <= 0) {
                    n && (e._zTime = n);
                    return
                }
            } else if (u && m && !p) {
                if (n && (a = !1),
                O = Ct({
                    overwrite: !1,
                    data: "isFromStart",
                    lazy: a && !p && ht(l),
                    immediateRender: a,
                    stagger: 0,
                    parent: E
                }, A),
                Z && (O[q.prop] = Z),
                In(e._startAt = Fe.set(b, O)),
                e._startAt._dp = 0,
                e._startAt._sat = e,
                n < 0 && (We ? e._startAt.revert(Vs) : e._startAt.render(-1, !0)),
                e._zTime = n,
                !a)
                    t(e._startAt, tt, tt);
                else if (!n)
                    return
            }
            for (e._pt = e._ptCache = 0,
            l = m && ht(l) || l && !m,
            T = 0; T < b.length; T++) {
                if (P = b[T],
                X = P._gsap || Sa(b)[T]._gsap,
                e._ptLookup[T] = _e = {},
                Po[X.id] && On.length && rr(),
                ue = S === b ? T : S.indexOf(P),
                q && (Q = new q).init(P, Z || A, e, ue, S) !== !1 && (e._pt = C = new gt(e._pt,P,Q.name,0,1,Q.render,Q,0,Q.priority),
                Q._props.forEach(function(te) {
                    _e[te] = C
                }),
                Q.priority && (W = 1)),
                !q || Z)
                    for (O in A)
                        Et[O] && (Q = Yf(O, A, e, ue, P, S)) ? Q.priority && (W = 1) : _e[O] = C = Oa.call(e, P, O, "get", A[O], ue, S, 0, s.stringFilter);
                e._op && e._op[T] && e.kill(P, e._op[T]),
                v && e._pt && ($n = e,
                Se.killTweensOf(P, _e, e.globalTime(n)),
                J = !e.parent,
                $n = 0),
                e._pt && l && (Po[X.id] = 1)
            }
            W && td(e),
            e._onInit && e._onInit(e)
        }
        e._onUpdate = c,
        e._initted = (!e._op || e._pt) && !J,
        d && n <= 0 && y.render(tn, !0, !0)
    }, Pg = function(e, n, i, s, r, o, a, l) {
        var c = (e._pt && e._ptCache || (e._ptCache = {}))[n], u, f, d, _;
        if (!c)
            for (c = e._ptCache[n] = [],
            d = e._ptLookup,
            _ = e._targets.length; _--; ) {
                if (u = d[_][n],
                u && u.d && u.d._pt)
                    for (u = u.d._pt; u && u.p !== n && u.fp !== n; )
                        u = u._next;
                if (!u)
                    return No = 1,
                    e.vars[n] = "+=0",
                    Pa(e, a),
                    No = 0,
                    l ? _s(n + " not eligible for reset") : 1;
                c.push(u)
            }
        for (_ = c.length; _--; )
            f = c[_],
            u = f._pt || f,
            u.s = (s || s === 0) && !r ? s : u.s + (s || 0) + o * u.c,
            u.c = i - u.s,
            f.e && (f.e = Ne(i) + et(f.e)),
            f.b && (f.b = u.s + et(f.b))
    }, xg = function(e, n) {
        var i = e[0] ? ni(e[0]).harness : 0, s = i && i.aliases, r, o, a, l;
        if (!s)
            return n;
        r = xi({}, n);
        for (o in s)
            if (o in r)
                for (l = s[o].split(","),
                a = l.length; a--; )
                    r[l[a]] = r[o];
        return r
    }, Ig = function(e, n, i, s) {
        var r = n.ease || s || "power1.inOut", o, a;
        if (nt(n))
            a = i[e] || (i[e] = []),
            n.forEach(function(l, c) {
                return a.push({
                    t: c / (n.length - 1) * 100,
                    v: l,
                    e: r
                })
            });
        else
            for (o in n)
                a = i[o] || (i[o] = []),
                o === "ease" || a.push({
                    t: parseFloat(e),
                    v: n[o],
                    e: r
                })
    }, rs = function(e, n, i, s, r) {
        return Le(e) ? e.call(n, i, s, r) : He(e) && ~e.indexOf("random(") ? ms(e) : e
    }, Xf = wa + "repeat,repeatDelay,yoyo,repeatRefresh,yoyoEase,autoRevert", Jf = {};
    mt(Xf + ",id,stagger,delay,duration,paused,scrollTrigger", function(t) {
        return Jf[t] = 1
    });
    var Fe = function(t) {
        hf(e, t);
        function e(i, s, r, o) {
            var a;
            typeof s == "number" && (r.duration = s,
            s = r,
            r = null),
            a = t.call(this, o ? s : is(s)) || this;
            var l = a.vars, c = l.duration, u = l.delay, f = l.immediateRender, d = l.stagger, _ = l.overwrite, m = l.keyframes, p = l.defaults, b = l.scrollTrigger, E = l.yoyoEase, S = s.parent || Se, v = (nt(i) || gf(i) ? mn(i[0]) : "length"in s) ? [i] : Nt(i), y, A, T, O, C, P, W, X;
            if (a._targets = v.length ? Sa(v) : _s("GSAP target " + i + " not found. https://gsap.com", !xt.nullTargetWarn) || [],
            a._ptLookup = [],
            a._overwrite = _,
            m || d || Ns(c) || Ns(u)) {
                if (s = a.vars,
                y = a.timeline = new ut({
                    data: "nested",
                    defaults: p || {},
                    targets: S && S.data === "nested" ? S.vars.targets : v
                }),
                y.kill(),
                y.parent = y._dp = dn(a),
                y._start = 0,
                d || Ns(c) || Ns(u)) {
                    if (O = v.length,
                    W = d && Nf(d),
                    rn(d))
                        for (C in d)
                            ~Xf.indexOf(C) && (X || (X = {}),
                            X[C] = d[C]);
                    for (A = 0; A < O; A++)
                        T = or(s, Jf),
                        T.stagger = 0,
                        E && (T.yoyoEase = E),
                        X && xi(T, X),
                        P = v[A],
                        T.duration = +rs(c, dn(a), A, P, v),
                        T.delay = (+rs(u, dn(a), A, P, v) || 0) - a._delay,
                        !d && O === 1 && T.delay && (a._delay = u = T.delay,
                        a._start += u,
                        T.delay = 0),
                        y.to(P, T, W ? W(A, P, v) : 0),
                        y._ease = le.none;
                    y.duration() ? c = u = 0 : a.timeline = 0
                } else if (m) {
                    is(Ct(y.vars.defaults, {
                        ease: "none"
                    })),
                    y._ease = si(m.ease || s.ease || "none");
                    var q = 0, Q, _e, ue;
                    if (nt(m))
                        m.forEach(function(Z) {
                            return y.to(v, Z, ">")
                        }),
                        y.duration();
                    else {
                        T = {};
                        for (C in m)
                            C === "ease" || C === "easeEach" || Ig(C, m[C], T, m.easeEach);
                        for (C in T)
                            for (Q = T[C].sort(function(Z, J) {
                                return Z.t - J.t
                            }),
                            q = 0,
                            A = 0; A < Q.length; A++)
                                _e = Q[A],
                                ue = {
                                    ease: _e.e,
                                    duration: (_e.t - (A ? Q[A - 1].t : 0)) / 100 * c
                                },
                                ue[C] = _e.v,
                                y.to(v, ue, q),
                                q += ue.duration;
                        y.duration() < c && y.to({}, {
                            duration: c - y.duration()
                        })
                    }
                }
                c || a.duration(c = y.duration())
            } else
                a.timeline = 0;
            return _ === !0 && !va && ($n = dn(a),
            Se.killTweensOf(v),
            $n = 0),
            Jt(S, dn(a), r),
            s.reversed && a.reverse(),
            s.paused && a.paused(!0),
            (f || !c && !m && a._start === Ge(S._time) && ht(f) && lg(dn(a)) && S.data !== "nested") && (a._tTime = -1e-8,
            a.render(Math.max(0, -u) || 0)),
            b && If(dn(a), b),
            a
        }
        var n = e.prototype;
        return n.render = function(s, r, o) {
            var a = this._time, l = this._tDur, c = this._dur, u = s < 0, f = s > l - tt && !u ? l : s < tt ? 0 : s, d, _, m, p, b, E, S, v, y;
            if (!c)
                ug(this, s, r, o);
            else if (f !== this._tTime || !s || o || !this._initted && this._tTime || this._startAt && this._zTime < 0 !== u || this._lazy) {
                if (d = f,
                v = this.timeline,
                this._repeat) {
                    if (p = c + this._rDelay,
                    this._repeat < -1 && u)
                        return this.totalTime(p * 100 + s, r, o);
                    if (d = Ge(f % p),
                    f === l ? (m = this._repeat,
                    d = c) : (b = Ge(f / p),
                    m = ~~b,
                    m && m === b ? (d = c,
                    m--) : d > c && (d = c)),
                    E = this._yoyo && m & 1,
                    E && (y = this._yEase,
                    d = c - d),
                    b = Ii(this._tTime, p),
                    d === a && !o && this._initted && m === b)
                        return this._tTime = f,
                        this;
                    m !== b && (v && this._yEase && Kf(v, E),
                    this.vars.repeatRefresh && !E && !this._lock && d !== p && this._initted && (this._lock = o = 1,
                    this.render(Ge(p * m), !0).invalidate()._lock = 0))
                }
                if (!this._initted) {
                    if (Cf(this, u ? s : d, o, r, f))
                        return this._tTime = 0,
                        this;
                    if (a !== this._time && !(o && this.vars.repeatRefresh && m !== b))
                        return this;
                    if (c !== this._dur)
                        return this.render(s, r, o)
                }
                if (this._tTime = f,
                this._time = d,
                !this._act && this._ts && (this._act = 1,
                this._lazy = 0),
                this.ratio = S = (y || this._ease)(d / c),
                this._from && (this.ratio = S = 1 - S),
                d && !a && !r && !m && (Ot(this, "onStart"),
                this._tTime !== f))
                    return this;
                for (_ = this._pt; _; )
                    _.r(S, _.d),
                    _ = _._next;
                v && v.render(s < 0 ? s : v._dur * v._ease(d / this._dur), r, o) || this._startAt && (this._zTime = s),
                this._onUpdate && !r && (u && xo(this, s, r, o),
                Ot(this, "onUpdate")),
                this._repeat && m !== b && this.vars.onRepeat && !r && this.parent && Ot(this, "onRepeat"),
                (f === this._tDur || !f) && this._tTime === f && (u && !this._onUpdate && xo(this, s, !0, !0),
                (s || !c) && (f === this._tDur && this._ts > 0 || !f && this._ts < 0) && In(this, 1),
                !r && !(u && !a) && (f || a || E) && (Ot(this, f === l ? "onComplete" : "onReverseComplete", !0),
                this._prom && !(f < l && this.timeScale() > 0) && this._prom()))
            }
            return this
        }
        ,
        n.targets = function() {
            return this._targets
        }
        ,
        n.invalidate = function(s) {
            return (!s || !this.vars.runBackwards) && (this._startAt = 0),
            this._pt = this._op = this._onUpdate = this._lazy = this.ratio = 0,
            this._ptLookup = [],
            this.timeline && this.timeline.invalidate(s),
            t.prototype.invalidate.call(this, s)
        }
        ,
        n.resetTo = function(s, r, o, a, l) {
            gs || wt.wake(),
            this._ts || this.play();
            var c = Math.min(this._dur, (this._dp._time - this._start) * this._ts), u;
            return this._initted || Pa(this, c),
            u = this._ease(c / this._dur),
            Pg(this, s, r, o, a, u, c, l) ? this.resetTo(s, r, o, a, 1) : (Or(this, 0),
            this.parent || Pf(this._dp, this, "_first", "_last", this._dp._sort ? "_start" : 0),
            this.render(0))
        }
        ,
        n.kill = function(s, r) {
            if (r === void 0 && (r = "all"),
            !s && (!r || r === "all"))
                return this._lazy = this._pt = 0,
                this.parent ? qi(this) : this.scrollTrigger && this.scrollTrigger.kill(!!We),
                this;
            if (this.timeline) {
                var o = this.timeline.totalDuration();
                return this.timeline.killTweensOf(s, r, $n && $n.vars.overwrite !== !0)._first || qi(this),
                this.parent && o !== this.timeline.totalDuration() && Ci(this, this._dur * this.timeline._tDur / o, 0, 1),
                this
            }
            var a = this._targets, l = s ? Nt(s) : a, c = this._ptLookup, u = this._pt, f, d, _, m, p, b, E;
            if ((!r || r === "all") && og(a, l))
                return r === "all" && (this._pt = 0),
                qi(this);
            for (f = this._op = this._op || [],
            r !== "all" && (He(r) && (p = {},
            mt(r, function(S) {
                return p[S] = 1
            }),
            r = p),
            r = xg(a, r)),
            E = a.length; E--; )
                if (~l.indexOf(a[E])) {
                    d = c[E],
                    r === "all" ? (f[E] = r,
                    m = d,
                    _ = {}) : (_ = f[E] = f[E] || {},
                    m = r);
                    for (p in m)
                        b = d && d[p],
                        b && ((!("kill"in b.d) || b.d.kill(p) === !0) && wr(this, b, "_pt"),
                        delete d[p]),
                        _ !== "all" && (_[p] = 1)
                }
            return this._initted && !this._pt && u && qi(this),
            this
        }
        ,
        e.to = function(s, r) {
            return new e(s,r,arguments[2])
        }
        ,
        e.from = function(s, r) {
            return ss(1, arguments)
        }
        ,
        e.delayedCall = function(s, r, o, a) {
            return new e(r,0,{
                immediateRender: !1,
                lazy: !1,
                overwrite: !1,
                delay: s,
                onComplete: r,
                onReverseComplete: r,
                onCompleteParams: o,
                onReverseCompleteParams: o,
                callbackScope: a
            })
        }
        ,
        e.fromTo = function(s, r, o) {
            return ss(2, arguments)
        }
        ,
        e.set = function(s, r) {
            return r.duration = 0,
            r.repeatDelay || (r.repeat = 0),
            new e(s,r)
        }
        ,
        e.killTweensOf = function(s, r, o) {
            return Se.killTweensOf(s, r, o)
        }
        ,
        e
    }(bs);
    Ct(Fe.prototype, {
        _targets: [],
        _lazy: 0,
        _startAt: 0,
        _op: 0,
        _onInit: 0
    });
    mt("staggerTo,staggerFrom,staggerFromTo", function(t) {
        Fe[t] = function() {
            var e = new ut
              , n = Co.call(arguments, 0);
            return n.splice(t === "staggerFromTo" ? 5 : 4, 0, 0),
            e[t].apply(e, n)
        }
    });
    var xa = function(e, n, i) {
        return e[n] = i
    }
      , Qf = function(e, n, i) {
        return e[n](i)
    }
      , Cg = function(e, n, i, s) {
        return e[n](s.fp, i)
    }
      , Rg = function(e, n, i) {
        return e.setAttribute(n, i)
    }
      , Ia = function(e, n) {
        return Le(e[n]) ? Qf : ka(e[n]) && e.setAttribute ? Rg : xa
    }
      , Zf = function(e, n) {
        return n.set(n.t, n.p, Math.round((n.s + n.c * e) * 1e6) / 1e6, n)
    }
      , Lg = function(e, n) {
        return n.set(n.t, n.p, !!(n.s + n.c * e), n)
    }
      , ed = function(e, n) {
        var i = n._pt
          , s = "";
        if (!e && n.b)
            s = n.b;
        else if (e === 1 && n.e)
            s = n.e;
        else {
            for (; i; )
                s = i.p + (i.m ? i.m(i.s + i.c * e) : Math.round((i.s + i.c * e) * 1e4) / 1e4) + s,
                i = i._next;
            s += n.c
        }
        n.set(n.t, n.p, s, n)
    }
      , Ca = function(e, n) {
        for (var i = n._pt; i; )
            i.r(e, i.d),
            i = i._next
    }
      , Ng = function(e, n, i, s) {
        for (var r = this._pt, o; r; )
            o = r._next,
            r.p === s && r.modifier(e, n, i),
            r = o
    }
      , Dg = function(e) {
        for (var n = this._pt, i, s; n; )
            s = n._next,
            n.p === e && !n.op || n.op === e ? wr(this, n, "_pt") : n.dep || (i = 1),
            n = s;
        return !i
    }
      , Mg = function(e, n, i, s) {
        s.mSet(e, n, s.m.call(s.tween, i, s.mt), s)
    }
      , td = function(e) {
        for (var n = e._pt, i, s, r, o; n; ) {
            for (i = n._next,
            s = r; s && s.pr > n.pr; )
                s = s._next;
            (n._prev = s ? s._prev : o) ? n._prev._next = n : r = n,
            (n._next = s) ? s._prev = n : o = n,
            n = i
        }
        e._pt = r
    }
      , gt = function() {
        function t(n, i, s, r, o, a, l, c, u) {
            this.t = i,
            this.s = r,
            this.c = o,
            this.p = s,
            this.r = a || Zf,
            this.d = l || this,
            this.set = c || xa,
            this.pr = u || 0,
            this._next = n,
            n && (n._prev = this)
        }
        var e = t.prototype;
        return e.modifier = function(i, s, r) {
            this.mSet = this.mSet || this.set,
            this.set = Mg,
            this.m = i,
            this.mt = r,
            this.tween = s
        }
        ,
        t
    }();
    mt(wa + "parent,duration,ease,delay,overwrite,runBackwards,startAt,yoyo,immediateRender,repeat,repeatDelay,data,paused,reversed,lazy,callbackScope,stringFilter,id,yoyoEase,stagger,inherit,repeatRefresh,keyframes,autoRevert,scrollTrigger", function(t) {
        return Ea[t] = 1
    });
    It.TweenMax = It.TweenLite = Fe;
    It.TimelineLite = It.TimelineMax = ut;
    Se = new ut({
        sortChildren: !1,
        defaults: Pi,
        autoRemoveChildren: !0,
        id: "root",
        smoothChildTiming: !0
    });
    xt.stringFilter = Vf;
    var ri = []
      , Ks = {}
      , Fg = []
      , zl = 0
      , Gg = 0
      , no = function(e) {
        return (Ks[e] || Fg).map(function(n) {
            return n()
        })
    }
      , Do = function() {
        var e = Date.now()
          , n = [];
        e - zl > 2 && (no("matchMediaInit"),
        ri.forEach(function(i) {
            var s = i.queries, r = i.conditions, o, a, l, c;
            for (a in s)
                o = Yt.matchMedia(s[a]).matches,
                o && (l = 1),
                o !== r[a] && (r[a] = o,
                c = 1);
            c && (i.revert(),
            l && n.push(i))
        }),
        no("matchMediaRevert"),
        n.forEach(function(i) {
            return i.onMatch(i, function(s) {
                return i.add(null, s)
            })
        }),
        zl = e,
        no("matchMedia"))
    }
      , nd = function() {
        function t(n, i) {
            this.selector = i && Ro(i),
            this.data = [],
            this._r = [],
            this.isReverted = !1,
            this.id = Gg++,
            n && this.add(n)
        }
        var e = t.prototype;
        return e.add = function(i, s, r) {
            Le(i) && (r = s,
            s = i,
            i = Le);
            var o = this
              , a = function() {
                var c = Ee, u = o.selector, f;
                return c && c !== o && c.data.push(o),
                r && (o.selector = Ro(r)),
                Ee = o,
                f = s.apply(o, arguments),
                Le(f) && o._r.push(f),
                Ee = c,
                o.selector = u,
                o.isReverted = !1,
                f
            };
            return o.last = a,
            i === Le ? a(o, function(l) {
                return o.add(null, l)
            }) : i ? o[i] = a : a
        }
        ,
        e.ignore = function(i) {
            var s = Ee;
            Ee = null,
            i(this),
            Ee = s
        }
        ,
        e.getTweens = function() {
            var i = [];
            return this.data.forEach(function(s) {
                return s instanceof t ? i.push.apply(i, s.getTweens()) : s instanceof Fe && !(s.parent && s.parent.data === "nested") && i.push(s)
            }),
            i
        }
        ,
        e.clear = function() {
            this._r.length = this.data.length = 0
        }
        ,
        e.kill = function(i, s) {
            var r = this;
            if (i ? function() {
                for (var a = r.getTweens(), l = r.data.length, c; l--; )
                    c = r.data[l],
                    c.data === "isFlip" && (c.revert(),
                    c.getChildren(!0, !0, !1).forEach(function(u) {
                        return a.splice(a.indexOf(u), 1)
                    }));
                for (a.map(function(u) {
                    return {
                        g: u._dur || u._delay || u._sat && !u._sat.vars.immediateRender ? u.globalTime(0) : -1 / 0,
                        t: u
                    }
                }).sort(function(u, f) {
                    return f.g - u.g || -1 / 0
                }).forEach(function(u) {
                    return u.t.revert(i)
                }),
                l = r.data.length; l--; )
                    c = r.data[l],
                    c instanceof ut ? c.data !== "nested" && (c.scrollTrigger && c.scrollTrigger.revert(),
                    c.kill()) : !(c instanceof Fe) && c.revert && c.revert(i);
                r._r.forEach(function(u) {
                    return u(i, r)
                }),
                r.isReverted = !0
            }() : this.data.forEach(function(a) {
                return a.kill && a.kill()
            }),
            this.clear(),
            s)
                for (var o = ri.length; o--; )
                    ri[o].id === this.id && ri.splice(o, 1)
        }
        ,
        e.revert = function(i) {
            this.kill(i || {})
        }
        ,
        t
    }()
      , Ug = function() {
        function t(n) {
            this.contexts = [],
            this.scope = n,
            Ee && Ee.data.push(this)
        }
        var e = t.prototype;
        return e.add = function(i, s, r) {
            rn(i) || (i = {
                matches: i
            });
            var o = new nd(0,r || this.scope), a = o.conditions = {}, l, c, u;
            Ee && !o.selector && (o.selector = Ee.selector),
            this.contexts.push(o),
            s = o.add("onMatch", s),
            o.queries = i;
            for (c in i)
                c === "all" ? u = 1 : (l = Yt.matchMedia(i[c]),
                l && (ri.indexOf(o) < 0 && ri.push(o),
                (a[c] = l.matches) && (u = 1),
                l.addListener ? l.addListener(Do) : l.addEventListener("change", Do)));
            return u && s(o, function(f) {
                return o.add(null, f)
            }),
            this
        }
        ,
        e.revert = function(i) {
            this.kill(i || {})
        }
        ,
        e.kill = function(i) {
            this.contexts.forEach(function(s) {
                return s.kill(i, !0)
            })
        }
        ,
        t
    }()
      , lr = {
        registerPlugin: function() {
            for (var e = arguments.length, n = new Array(e), i = 0; i < e; i++)
                n[i] = arguments[i];
            n.forEach(function(s) {
                return jf(s)
            })
        },
        timeline: function(e) {
            return new ut(e)
        },
        getTweensOf: function(e, n) {
            return Se.getTweensOf(e, n)
        },
        getProperty: function(e, n, i, s) {
            He(e) && (e = Nt(e)[0]);
            var r = ni(e || {}).get
              , o = i ? Of : Sf;
            return i === "native" && (i = ""),
            e && (n ? o((Et[n] && Et[n].get || r)(e, n, i, s)) : function(a, l, c) {
                return o((Et[a] && Et[a].get || r)(e, a, l, c))
            }
            )
        },
        quickSetter: function(e, n, i) {
            if (e = Nt(e),
            e.length > 1) {
                var s = e.map(function(u) {
                    return yt.quickSetter(u, n, i)
                })
                  , r = s.length;
                return function(u) {
                    for (var f = r; f--; )
                        s[f](u)
                }
            }
            e = e[0] || {};
            var o = Et[n]
              , a = ni(e)
              , l = a.harness && (a.harness.aliases || {})[n] || n
              , c = o ? function(u) {
                var f = new o;
                yi._pt = 0,
                f.init(e, i ? u + i : u, yi, 0, [e]),
                f.render(1, f),
                yi._pt && Ca(1, yi)
            }
            : a.set(e, l);
            return o ? c : function(u) {
                return c(e, l, i ? u + i : u, a, 1)
            }
        },
        quickTo: function(e, n, i) {
            var s, r = yt.to(e, Ct((s = {},
            s[n] = "+=0.1",
            s.paused = !0,
            s.stagger = 0,
            s), i || {})), o = function(l, c, u) {
                return r.resetTo(n, l, c, u)
            };
            return o.tween = r,
            o
        },
        isTweening: function(e) {
            return Se.getTweensOf(e, !0).length > 0
        },
        defaults: function(e) {
            return e && e.ease && (e.ease = si(e.ease, Pi.ease)),
            Fl(Pi, e || {})
        },
        config: function(e) {
            return Fl(xt, e || {})
        },
        registerEffect: function(e) {
            var n = e.name
              , i = e.effect
              , s = e.plugins
              , r = e.defaults
              , o = e.extendTimeline;
            (s || "").split(",").forEach(function(a) {
                return a && !Et[a] && !It[a] && _s(n + " effect requires " + a + " plugin.")
            }),
            Qr[n] = function(a, l, c) {
                return i(Nt(a), Ct(l || {}, r), c)
            }
            ,
            o && (ut.prototype[n] = function(a, l, c) {
                return this.add(Qr[n](a, rn(l) ? l : (c = l) && {}, this), c)
            }
            )
        },
        registerEase: function(e, n) {
            le[e] = si(n)
        },
        parseEase: function(e, n) {
            return arguments.length ? si(e, n) : le
        },
        getById: function(e) {
            return Se.getById(e)
        },
        exportRoot: function(e, n) {
            e === void 0 && (e = {});
            var i = new ut(e), s, r;
            for (i.smoothChildTiming = ht(e.smoothChildTiming),
            Se.remove(i),
            i._dp = 0,
            i._time = i._tTime = Se._time,
            s = Se._first; s; )
                r = s._next,
                (n || !(!s._dur && s instanceof Fe && s.vars.onComplete === s._targets[0])) && Jt(i, s, s._start - s._delay),
                s = r;
            return Jt(Se, i, 0),
            i
        },
        context: function(e, n) {
            return e ? new nd(e,n) : Ee
        },
        matchMedia: function(e) {
            return new Ug(e)
        },
        matchMediaRefresh: function() {
            return ri.forEach(function(e) {
                var n = e.conditions, i, s;
                for (s in n)
                    n[s] && (n[s] = !1,
                    i = 1);
                i && e.revert()
            }) || Do()
        },
        addEventListener: function(e, n) {
            var i = Ks[e] || (Ks[e] = []);
            ~i.indexOf(n) || i.push(n)
        },
        removeEventListener: function(e, n) {
            var i = Ks[e]
              , s = i && i.indexOf(n);
            s >= 0 && i.splice(s, 1)
        },
        utils: {
            wrap: bg,
            wrapYoyo: yg,
            distribute: Nf,
            random: Mf,
            snap: Df,
            normalize: gg,
            getUnit: et,
            clamp: pg,
            splitColor: Bf,
            toArray: Nt,
            selector: Ro,
            mapRange: Gf,
            pipe: hg,
            unitize: mg,
            interpolate: vg,
            shuffle: Lf
        },
        install: Af,
        effects: Qr,
        ticker: wt,
        updateRoot: ut.updateRoot,
        plugins: Et,
        globalTimeline: Se,
        core: {
            PropTween: gt,
            globals: $f,
            Tween: Fe,
            Timeline: ut,
            Animation: bs,
            getCache: ni,
            _removeLinkedListItem: wr,
            reverting: function() {
                return We
            },
            context: function(e) {
                return e && Ee && (Ee.data.push(e),
                e._ctx = Ee),
                Ee
            },
            suppressOverwrites: function(e) {
                return va = e
            }
        }
    };
    mt("to,from,fromTo,delayedCall,set,killTweensOf", function(t) {
        return lr[t] = Fe[t]
    });
    wt.add(ut.updateRoot);
    yi = lr.to({}, {
        duration: 0
    });
    var jg = function(e, n) {
        for (var i = e._pt; i && i.p !== n && i.op !== n && i.fp !== n; )
            i = i._next;
        return i
    }
      , Bg = function(e, n) {
        var i = e._targets, s, r, o;
        for (s in n)
            for (r = i.length; r--; )
                o = e._ptLookup[r][s],
                o && (o = o.d) && (o._pt && (o = jg(o, s)),
                o && o.modifier && o.modifier(n[s], e, i[r], s))
    }
      , io = function(e, n) {
        return {
            name: e,
            rawVars: 1,
            init: function(s, r, o) {
                o._onInit = function(a) {
                    var l, c;
                    if (He(r) && (l = {},
                    mt(r, function(u) {
                        return l[u] = 1
                    }),
                    r = l),
                    n) {
                        l = {};
                        for (c in r)
                            l[c] = n(r[c]);
                        r = l
                    }
                    Bg(a, r)
                }
            }
        }
    }
      , yt = lr.registerPlugin({
        name: "attr",
        init: function(e, n, i, s, r) {
            var o, a, l;
            this.tween = i;
            for (o in n)
                l = e.getAttribute(o) || "",
                a = this.add(e, "setAttribute", (l || 0) + "", n[o], s, r, 0, 0, o),
                a.op = o,
                a.b = l,
                this._props.push(o)
        },
        render: function(e, n) {
            for (var i = n._pt; i; )
                We ? i.set(i.t, i.p, i.b, i) : i.r(e, i.d),
                i = i._next
        }
    }, {
        name: "endArray",
        init: function(e, n) {
            for (var i = n.length; i--; )
                this.add(e, i, e[i] || 0, n[i], 0, 0, 0, 0, 0, 1)
        }
    }, io("roundProps", Lo), io("modifiers"), io("snap", Df)) || lr;
    Fe.version = ut.version = yt.version = "3.12.7";
    kf = 1;
    Aa() && Ri();
    le.Power0;
    le.Power1;
    le.Power2;
    le.Power3;
    le.Power4;
    le.Linear;
    le.Quad;
    le.Cubic;
    le.Quart;
    le.Quint;
    le.Strong;
    le.Elastic;
    le.Back;
    le.SteppedEase;
    le.Bounce;
    le.Sine;
    le.Expo;
    le.Circ;
    /*!
 * CSSPlugin 3.12.7
 * https://gsap.com
 *
 * Copyright 2008-2025, GreenSock. All rights reserved.
 * Subject to the terms at https://gsap.com/standard-license or for
 * Club GSAP members, the agreement issued with that membership.
 * @author: Jack Doyle, jack@greensock.com
*/
    var Vl, Tn, wi, Ra, Zn, Hl, La, zg = function() {
        return typeof window != "undefined"
    }, gn = {}, Jn = 180 / Math.PI, Si = Math.PI / 180, _i = Math.atan2, Kl = 1e8, Na = /([A-Z])/g, Vg = /(left|right|width|margin|padding|x)/i, Hg = /[\s,\(]\S/, Qt = {
        autoAlpha: "opacity,visibility",
        scale: "scaleX,scaleY",
        alpha: "opacity"
    }, Mo = function(e, n) {
        return n.set(n.t, n.p, Math.round((n.s + n.c * e) * 1e4) / 1e4 + n.u, n)
    }, Kg = function(e, n) {
        return n.set(n.t, n.p, e === 1 ? n.e : Math.round((n.s + n.c * e) * 1e4) / 1e4 + n.u, n)
    }, qg = function(e, n) {
        return n.set(n.t, n.p, e ? Math.round((n.s + n.c * e) * 1e4) / 1e4 + n.u : n.b, n)
    }, Wg = function(e, n) {
        var i = n.s + n.c * e;
        n.set(n.t, n.p, ~~(i + (i < 0 ? -.5 : .5)) + n.u, n)
    }, id = function(e, n) {
        return n.set(n.t, n.p, e ? n.e : n.b, n)
    }, sd = function(e, n) {
        return n.set(n.t, n.p, e !== 1 ? n.b : n.e, n)
    }, Yg = function(e, n, i) {
        return e.style[n] = i
    }, Xg = function(e, n, i) {
        return e.style.setProperty(n, i)
    }, Jg = function(e, n, i) {
        return e._gsap[n] = i
    }, Qg = function(e, n, i) {
        return e._gsap.scaleX = e._gsap.scaleY = i
    }, Zg = function(e, n, i, s, r) {
        var o = e._gsap;
        o.scaleX = o.scaleY = i,
        o.renderTransform(r, o)
    }, eb = function(e, n, i, s, r) {
        var o = e._gsap;
        o[n] = i,
        o.renderTransform(r, o)
    }, Oe = "transform", bt = Oe + "Origin", tb = function t(e, n) {
        var i = this
          , s = this.target
          , r = s.style
          , o = s._gsap;
        if (e in gn && r) {
            if (this.tfm = this.tfm || {},
            e !== "transform")
                e = Qt[e] || e,
                ~e.indexOf(",") ? e.split(",").forEach(function(a) {
                    return i.tfm[a] = pn(s, a)
                }) : this.tfm[e] = o.x ? o[e] : pn(s, e),
                e === bt && (this.tfm.zOrigin = o.zOrigin);
            else
                return Qt.transform.split(",").forEach(function(a) {
                    return t.call(i, a, n)
                });
            if (this.props.indexOf(Oe) >= 0)
                return;
            o.svg && (this.svgo = s.getAttribute("data-svg-origin"),
            this.props.push(bt, n, "")),
            e = Oe
        }
        (r || n) && this.props.push(e, n, r[e])
    }, rd = function(e) {
        e.translate && (e.removeProperty("translate"),
        e.removeProperty("scale"),
        e.removeProperty("rotate"))
    }, nb = function() {
        var e = this.props, n = this.target, i = n.style, s = n._gsap, r, o;
        for (r = 0; r < e.length; r += 3)
            e[r + 1] ? e[r + 1] === 2 ? n[e[r]](e[r + 2]) : n[e[r]] = e[r + 2] : e[r + 2] ? i[e[r]] = e[r + 2] : i.removeProperty(e[r].substr(0, 2) === "--" ? e[r] : e[r].replace(Na, "-$1").toLowerCase());
        if (this.tfm) {
            for (o in this.tfm)
                s[o] = this.tfm[o];
            s.svg && (s.renderTransform(),
            n.setAttribute("data-svg-origin", this.svgo || "")),
            r = La(),
            (!r || !r.isStart) && !i[Oe] && (rd(i),
            s.zOrigin && i[bt] && (i[bt] += " " + s.zOrigin + "px",
            s.zOrigin = 0,
            s.renderTransform()),
            s.uncache = 1)
        }
    }, od = function(e, n) {
        var i = {
            target: e,
            props: [],
            revert: nb,
            save: tb
        };
        return e._gsap || yt.core.getCache(e),
        n && e.style && e.nodeType && n.split(",").forEach(function(s) {
            return i.save(s)
        }),
        i
    }, ad, Fo = function(e, n) {
        var i = Tn.createElementNS ? Tn.createElementNS((n || "http://www.w3.org/1999/xhtml").replace(/^https/, "http"), e) : Tn.createElement(e);
        return i && i.style ? i : Tn.createElement(e)
    }, nn = function t(e, n, i) {
        var s = getComputedStyle(e);
        return s[n] || s.getPropertyValue(n.replace(Na, "-$1").toLowerCase()) || s.getPropertyValue(n) || !i && t(e, Li(n) || n, 1) || ""
    }, ql = "O,Moz,ms,Ms,Webkit".split(","), Li = function(e, n, i) {
        var s = n || Zn
          , r = s.style
          , o = 5;
        if (e in r && !i)
            return e;
        for (e = e.charAt(0).toUpperCase() + e.substr(1); o-- && !(ql[o] + e in r); )
            ;
        return o < 0 ? null : (o === 3 ? "ms" : o >= 0 ? ql[o] : "") + e
    }, Go = function() {
        zg() && window.document && (Vl = window,
        Tn = Vl.document,
        wi = Tn.documentElement,
        Zn = Fo("div") || {
            style: {}
        },
        Fo("div"),
        Oe = Li(Oe),
        bt = Oe + "Origin",
        Zn.style.cssText = "border-width:0;line-height:0;position:absolute;padding:0",
        ad = !!Li("perspective"),
        La = yt.core.reverting,
        Ra = 1)
    }, Wl = function(e) {
        var n = e.ownerSVGElement, i = Fo("svg", n && n.getAttribute("xmlns") || "http://www.w3.org/2000/svg"), s = e.cloneNode(!0), r;
        s.style.display = "block",
        i.appendChild(s),
        wi.appendChild(i);
        try {
            r = s.getBBox()
        } catch (o) {}
        return i.removeChild(s),
        wi.removeChild(i),
        r
    }, Yl = function(e, n) {
        for (var i = n.length; i--; )
            if (e.hasAttribute(n[i]))
                return e.getAttribute(n[i])
    }, ld = function(e) {
        var n, i;
        try {
            n = e.getBBox()
        } catch (s) {
            n = Wl(e),
            i = 1
        }
        return n && (n.width || n.height) || i || (n = Wl(e)),
        n && !n.width && !n.x && !n.y ? {
            x: +Yl(e, ["x", "cx", "x1"]) || 0,
            y: +Yl(e, ["y", "cy", "y1"]) || 0,
            width: 0,
            height: 0
        } : n
    }, cd = function(e) {
        return !!(e.getCTM && (!e.parentNode || e.ownerSVGElement) && ld(e))
    }, ci = function(e, n) {
        if (n) {
            var i = e.style, s;
            n in gn && n !== bt && (n = Oe),
            i.removeProperty ? (s = n.substr(0, 2),
            (s === "ms" || n.substr(0, 6) === "webkit") && (n = "-" + n),
            i.removeProperty(s === "--" ? n : n.replace(Na, "-$1").toLowerCase())) : i.removeAttribute(n)
        }
    }, En = function(e, n, i, s, r, o) {
        var a = new gt(e._pt,n,i,0,1,o ? sd : id);
        return e._pt = a,
        a.b = s,
        a.e = r,
        e._props.push(i),
        a
    }, Xl = {
        deg: 1,
        rad: 1,
        turn: 1
    }, ib = {
        grid: 1,
        flex: 1
    }, Cn = function t(e, n, i, s) {
        var r = parseFloat(i) || 0, o = (i + "").trim().substr((r + "").length) || "px", a = Zn.style, l = Vg.test(n), c = e.tagName.toLowerCase() === "svg", u = (c ? "client" : "offset") + (l ? "Width" : "Height"), f = 100, d = s === "px", _ = s === "%", m, p, b, E;
        if (s === o || !r || Xl[s] || Xl[o])
            return r;
        if (o !== "px" && !d && (r = t(e, n, i, "px")),
        E = e.getCTM && cd(e),
        (_ || o === "%") && (gn[n] || ~n.indexOf("adius")))
            return m = E ? e.getBBox()[l ? "width" : "height"] : e[u],
            Ne(_ ? r / m * f : r / 100 * m);
        if (a[l ? "width" : "height"] = f + (d ? o : s),
        p = s !== "rem" && ~n.indexOf("adius") || s === "em" && e.appendChild && !c ? e : e.parentNode,
        E && (p = (e.ownerSVGElement || {}).parentNode),
        (!p || p === Tn || !p.appendChild) && (p = Tn.body),
        b = p._gsap,
        b && _ && b.width && l && b.time === wt.time && !b.uncache)
            return Ne(r / b.width * f);
        if (_ && (n === "height" || n === "width")) {
            var S = e.style[n];
            e.style[n] = f + s,
            m = e[u],
            S ? e.style[n] = S : ci(e, n)
        } else
            (_ || o === "%") && !ib[nn(p, "display")] && (a.position = nn(e, "position")),
            p === e && (a.position = "static"),
            p.appendChild(Zn),
            m = Zn[u],
            p.removeChild(Zn),
            a.position = "absolute";
        return l && _ && (b = ni(p),
        b.time = wt.time,
        b.width = p[u]),
        Ne(d ? m * r / f : m && r ? f / m * r : 0)
    }, pn = function(e, n, i, s) {
        var r;
        return Ra || Go(),
        n in Qt && n !== "transform" && (n = Qt[n],
        ~n.indexOf(",") && (n = n.split(",")[0])),
        gn[n] && n !== "transform" ? (r = vs(e, s),
        r = n !== "transformOrigin" ? r[n] : r.svg ? r.origin : ur(nn(e, bt)) + " " + r.zOrigin + "px") : (r = e.style[n],
        (!r || r === "auto" || s || ~(r + "").indexOf("calc(")) && (r = cr[n] && cr[n](e, n, i) || nn(e, n) || Ef(e, n) || (n === "opacity" ? 1 : 0))),
        i && !~(r + "").trim().indexOf(" ") ? Cn(e, n, r, i) + i : r
    }, sb = function(e, n, i, s) {
        if (!i || i === "none") {
            var r = Li(n, e, 1)
              , o = r && nn(e, r, 1);
            o && o !== i ? (n = r,
            i = o) : n === "borderColor" && (i = nn(e, "borderTopColor"))
        }
        var a = new gt(this._pt,e.style,n,0,1,ed), l = 0, c = 0, u, f, d, _, m, p, b, E, S, v, y, A;
        if (a.b = i,
        a.e = s,
        i += "",
        s += "",
        s === "auto" && (p = e.style[n],
        e.style[n] = s,
        s = nn(e, n) || s,
        p ? e.style[n] = p : ci(e, n)),
        u = [i, s],
        Vf(u),
        i = u[0],
        s = u[1],
        d = i.match(bi) || [],
        A = s.match(bi) || [],
        A.length) {
            for (; f = bi.exec(s); )
                b = f[0],
                S = s.substring(l, f.index),
                m ? m = (m + 1) % 5 : (S.substr(-5) === "rgba(" || S.substr(-5) === "hsla(") && (m = 1),
                b !== (p = d[c++] || "") && (_ = parseFloat(p) || 0,
                y = p.substr((_ + "").length),
                b.charAt(1) === "=" && (b = Ei(_, b) + y),
                E = parseFloat(b),
                v = b.substr((E + "").length),
                l = bi.lastIndex - v.length,
                v || (v = v || xt.units[n] || y,
                l === s.length && (s += v,
                a.e += v)),
                y !== v && (_ = Cn(e, n, p, v) || 0),
                a._pt = {
                    _next: a._pt,
                    p: S || c === 1 ? S : ",",
                    s: _,
                    c: E - _,
                    m: m && m < 4 || n === "zIndex" ? Math.round : 0
                });
            a.c = l < s.length ? s.substring(l, s.length) : ""
        } else
            a.r = n === "display" && s === "none" ? sd : id;
        return yf.test(s) && (a.e = 0),
        this._pt = a,
        a
    }, Jl = {
        top: "0%",
        bottom: "100%",
        left: "0%",
        right: "100%",
        center: "50%"
    }, rb = function(e) {
        var n = e.split(" ")
          , i = n[0]
          , s = n[1] || "50%";
        return (i === "top" || i === "bottom" || s === "left" || s === "right") && (e = i,
        i = s,
        s = e),
        n[0] = Jl[i] || i,
        n[1] = Jl[s] || s,
        n.join(" ")
    }, ob = function(e, n) {
        if (n.tween && n.tween._time === n.tween._dur) {
            var i = n.t, s = i.style, r = n.u, o = i._gsap, a, l, c;
            if (r === "all" || r === !0)
                s.cssText = "",
                l = 1;
            else
                for (r = r.split(","),
                c = r.length; --c > -1; )
                    a = r[c],
                    gn[a] && (l = 1,
                    a = a === "transformOrigin" ? bt : Oe),
                    ci(i, a);
            l && (ci(i, Oe),
            o && (o.svg && i.removeAttribute("transform"),
            s.scale = s.rotate = s.translate = "none",
            vs(i, 1),
            o.uncache = 1,
            rd(s)))
        }
    }, cr = {
        clearProps: function(e, n, i, s, r) {
            if (r.data !== "isFromStart") {
                var o = e._pt = new gt(e._pt,n,i,0,0,ob);
                return o.u = s,
                o.pr = -10,
                o.tween = r,
                e._props.push(i),
                1
            }
        }
    }, ys = [1, 0, 0, 1, 0, 0], ud = {}, fd = function(e) {
        return e === "matrix(1, 0, 0, 1, 0, 0)" || e === "none" || !e
    }, Ql = function(e) {
        var n = nn(e, Oe);
        return fd(n) ? ys : n.substr(7).match(bf).map(Ne)
    }, Da = function(e, n) {
        var i = e._gsap || ni(e), s = e.style, r = Ql(e), o, a, l, c;
        return i.svg && e.getAttribute("transform") ? (l = e.transform.baseVal.consolidate().matrix,
        r = [l.a, l.b, l.c, l.d, l.e, l.f],
        r.join(",") === "1,0,0,1,0,0" ? ys : r) : (r === ys && !e.offsetParent && e !== wi && !i.svg && (l = s.display,
        s.display = "block",
        o = e.parentNode,
        (!o || !e.offsetParent && !e.getBoundingClientRect().width) && (c = 1,
        a = e.nextElementSibling,
        wi.appendChild(e)),
        r = Ql(e),
        l ? s.display = l : ci(e, "display"),
        c && (a ? o.insertBefore(e, a) : o ? o.appendChild(e) : wi.removeChild(e))),
        n && r.length > 6 ? [r[0], r[1], r[4], r[5], r[12], r[13]] : r)
    }, Uo = function(e, n, i, s, r, o) {
        var a = e._gsap, l = r || Da(e, !0), c = a.xOrigin || 0, u = a.yOrigin || 0, f = a.xOffset || 0, d = a.yOffset || 0, _ = l[0], m = l[1], p = l[2], b = l[3], E = l[4], S = l[5], v = n.split(" "), y = parseFloat(v[0]) || 0, A = parseFloat(v[1]) || 0, T, O, C, P;
        i ? l !== ys && (O = _ * b - m * p) && (C = y * (b / O) + A * (-p / O) + (p * S - b * E) / O,
        P = y * (-m / O) + A * (_ / O) - (_ * S - m * E) / O,
        y = C,
        A = P) : (T = ld(e),
        y = T.x + (~v[0].indexOf("%") ? y / 100 * T.width : y),
        A = T.y + (~(v[1] || v[0]).indexOf("%") ? A / 100 * T.height : A)),
        s || s !== !1 && a.smooth ? (E = y - c,
        S = A - u,
        a.xOffset = f + (E * _ + S * p) - E,
        a.yOffset = d + (E * m + S * b) - S) : a.xOffset = a.yOffset = 0,
        a.xOrigin = y,
        a.yOrigin = A,
        a.smooth = !!s,
        a.origin = n,
        a.originIsAbsolute = !!i,
        e.style[bt] = "0px 0px",
        o && (En(o, a, "xOrigin", c, y),
        En(o, a, "yOrigin", u, A),
        En(o, a, "xOffset", f, a.xOffset),
        En(o, a, "yOffset", d, a.yOffset)),
        e.setAttribute("data-svg-origin", y + " " + A)
    }, vs = function(e, n) {
        var i = e._gsap || new Wf(e);
        if ("x"in i && !n && !i.uncache)
            return i;
        var s = e.style, r = i.scaleX < 0, o = "px", a = "deg", l = getComputedStyle(e), c = nn(e, bt) || "0", u, f, d, _, m, p, b, E, S, v, y, A, T, O, C, P, W, X, q, Q, _e, ue, Z, J, te, Ie, ze, he, me, vt, G, K;
        return u = f = d = p = b = E = S = v = y = 0,
        _ = m = 1,
        i.svg = !!(e.getCTM && cd(e)),
        l.translate && ((l.translate !== "none" || l.scale !== "none" || l.rotate !== "none") && (s[Oe] = (l.translate !== "none" ? "translate3d(" + (l.translate + " 0 0").split(" ").slice(0, 3).join(", ") + ") " : "") + (l.rotate !== "none" ? "rotate(" + l.rotate + ") " : "") + (l.scale !== "none" ? "scale(" + l.scale.split(" ").join(",") + ") " : "") + (l[Oe] !== "none" ? l[Oe] : "")),
        s.scale = s.rotate = s.translate = "none"),
        O = Da(e, i.svg),
        i.svg && (i.uncache ? (te = e.getBBox(),
        c = i.xOrigin - te.x + "px " + (i.yOrigin - te.y) + "px",
        J = "") : J = !n && e.getAttribute("data-svg-origin"),
        Uo(e, J || c, !!J || i.originIsAbsolute, i.smooth !== !1, O)),
        A = i.xOrigin || 0,
        T = i.yOrigin || 0,
        O !== ys && (X = O[0],
        q = O[1],
        Q = O[2],
        _e = O[3],
        u = ue = O[4],
        f = Z = O[5],
        O.length === 6 ? (_ = Math.sqrt(X * X + q * q),
        m = Math.sqrt(_e * _e + Q * Q),
        p = X || q ? _i(q, X) * Jn : 0,
        S = Q || _e ? _i(Q, _e) * Jn + p : 0,
        S && (m *= Math.abs(Math.cos(S * Si))),
        i.svg && (u -= A - (A * X + T * Q),
        f -= T - (A * q + T * _e))) : (K = O[6],
        vt = O[7],
        ze = O[8],
        he = O[9],
        me = O[10],
        G = O[11],
        u = O[12],
        f = O[13],
        d = O[14],
        C = _i(K, me),
        b = C * Jn,
        C && (P = Math.cos(-C),
        W = Math.sin(-C),
        J = ue * P + ze * W,
        te = Z * P + he * W,
        Ie = K * P + me * W,
        ze = ue * -W + ze * P,
        he = Z * -W + he * P,
        me = K * -W + me * P,
        G = vt * -W + G * P,
        ue = J,
        Z = te,
        K = Ie),
        C = _i(-Q, me),
        E = C * Jn,
        C && (P = Math.cos(-C),
        W = Math.sin(-C),
        J = X * P - ze * W,
        te = q * P - he * W,
        Ie = Q * P - me * W,
        G = _e * W + G * P,
        X = J,
        q = te,
        Q = Ie),
        C = _i(q, X),
        p = C * Jn,
        C && (P = Math.cos(C),
        W = Math.sin(C),
        J = X * P + q * W,
        te = ue * P + Z * W,
        q = q * P - X * W,
        Z = Z * P - ue * W,
        X = J,
        ue = te),
        b && Math.abs(b) + Math.abs(p) > 359.9 && (b = p = 0,
        E = 180 - E),
        _ = Ne(Math.sqrt(X * X + q * q + Q * Q)),
        m = Ne(Math.sqrt(Z * Z + K * K)),
        C = _i(ue, Z),
        S = Math.abs(C) > 2e-4 ? C * Jn : 0,
        y = G ? 1 / (G < 0 ? -G : G) : 0),
        i.svg && (J = e.getAttribute("transform"),
        i.forceCSS = e.setAttribute("transform", "") || !fd(nn(e, Oe)),
        J && e.setAttribute("transform", J))),
        Math.abs(S) > 90 && Math.abs(S) < 270 && (r ? (_ *= -1,
        S += p <= 0 ? 180 : -180,
        p += p <= 0 ? 180 : -180) : (m *= -1,
        S += S <= 0 ? 180 : -180)),
        n = n || i.uncache,
        i.x = u - ((i.xPercent = u && (!n && i.xPercent || (Math.round(e.offsetWidth / 2) === Math.round(-u) ? -50 : 0))) ? e.offsetWidth * i.xPercent / 100 : 0) + o,
        i.y = f - ((i.yPercent = f && (!n && i.yPercent || (Math.round(e.offsetHeight / 2) === Math.round(-f) ? -50 : 0))) ? e.offsetHeight * i.yPercent / 100 : 0) + o,
        i.z = d + o,
        i.scaleX = Ne(_),
        i.scaleY = Ne(m),
        i.rotation = Ne(p) + a,
        i.rotationX = Ne(b) + a,
        i.rotationY = Ne(E) + a,
        i.skewX = S + a,
        i.skewY = v + a,
        i.transformPerspective = y + o,
        (i.zOrigin = parseFloat(c.split(" ")[2]) || !n && i.zOrigin || 0) && (s[bt] = ur(c)),
        i.xOffset = i.yOffset = 0,
        i.force3D = xt.force3D,
        i.renderTransform = i.svg ? lb : ad ? dd : ab,
        i.uncache = 0,
        i
    }, ur = function(e) {
        return (e = e.split(" "))[0] + " " + e[1]
    }, so = function(e, n, i) {
        var s = et(n);
        return Ne(parseFloat(n) + parseFloat(Cn(e, "x", i + "px", s))) + s
    }, ab = function(e, n) {
        n.z = "0px",
        n.rotationY = n.rotationX = "0deg",
        n.force3D = 0,
        dd(e, n)
    }, Kn = "0deg", Vi = "0px", qn = ") ", dd = function(e, n) {
        var i = n || this
          , s = i.xPercent
          , r = i.yPercent
          , o = i.x
          , a = i.y
          , l = i.z
          , c = i.rotation
          , u = i.rotationY
          , f = i.rotationX
          , d = i.skewX
          , _ = i.skewY
          , m = i.scaleX
          , p = i.scaleY
          , b = i.transformPerspective
          , E = i.force3D
          , S = i.target
          , v = i.zOrigin
          , y = ""
          , A = E === "auto" && e && e !== 1 || E === !0;
        if (v && (f !== Kn || u !== Kn)) {
            var T = parseFloat(u) * Si, O = Math.sin(T), C = Math.cos(T), P;
            T = parseFloat(f) * Si,
            P = Math.cos(T),
            o = so(S, o, O * P * -v),
            a = so(S, a, -Math.sin(T) * -v),
            l = so(S, l, C * P * -v + v)
        }
        b !== Vi && (y += "perspective(" + b + qn),
        (s || r) && (y += "translate(" + s + "%, " + r + "%) "),
        (A || o !== Vi || a !== Vi || l !== Vi) && (y += l !== Vi || A ? "translate3d(" + o + ", " + a + ", " + l + ") " : "translate(" + o + ", " + a + qn),
        c !== Kn && (y += "rotate(" + c + qn),
        u !== Kn && (y += "rotateY(" + u + qn),
        f !== Kn && (y += "rotateX(" + f + qn),
        (d !== Kn || _ !== Kn) && (y += "skew(" + d + ", " + _ + qn),
        (m !== 1 || p !== 1) && (y += "scale(" + m + ", " + p + qn),
        S.style[Oe] = y || "translate(0, 0)"
    }, lb = function(e, n) {
        var i = n || this, s = i.xPercent, r = i.yPercent, o = i.x, a = i.y, l = i.rotation, c = i.skewX, u = i.skewY, f = i.scaleX, d = i.scaleY, _ = i.target, m = i.xOrigin, p = i.yOrigin, b = i.xOffset, E = i.yOffset, S = i.forceCSS, v = parseFloat(o), y = parseFloat(a), A, T, O, C, P;
        l = parseFloat(l),
        c = parseFloat(c),
        u = parseFloat(u),
        u && (u = parseFloat(u),
        c += u,
        l += u),
        l || c ? (l *= Si,
        c *= Si,
        A = Math.cos(l) * f,
        T = Math.sin(l) * f,
        O = Math.sin(l - c) * -d,
        C = Math.cos(l - c) * d,
        c && (u *= Si,
        P = Math.tan(c - u),
        P = Math.sqrt(1 + P * P),
        O *= P,
        C *= P,
        u && (P = Math.tan(u),
        P = Math.sqrt(1 + P * P),
        A *= P,
        T *= P)),
        A = Ne(A),
        T = Ne(T),
        O = Ne(O),
        C = Ne(C)) : (A = f,
        C = d,
        T = O = 0),
        (v && !~(o + "").indexOf("px") || y && !~(a + "").indexOf("px")) && (v = Cn(_, "x", o, "px"),
        y = Cn(_, "y", a, "px")),
        (m || p || b || E) && (v = Ne(v + m - (m * A + p * O) + b),
        y = Ne(y + p - (m * T + p * C) + E)),
        (s || r) && (P = _.getBBox(),
        v = Ne(v + s / 100 * P.width),
        y = Ne(y + r / 100 * P.height)),
        P = "matrix(" + A + "," + T + "," + O + "," + C + "," + v + "," + y + ")",
        _.setAttribute("transform", P),
        S && (_.style[Oe] = P)
    }, cb = function(e, n, i, s, r) {
        var o = 360, a = He(r), l = parseFloat(r) * (a && ~r.indexOf("rad") ? Jn : 1), c = l - s, u = s + c + "deg", f, d;
        return a && (f = r.split("_")[1],
        f === "short" && (c %= o,
        c !== c % (o / 2) && (c += c < 0 ? o : -360)),
        f === "cw" && c < 0 ? c = (c + o * Kl) % o - ~~(c / o) * o : f === "ccw" && c > 0 && (c = (c - o * Kl) % o - ~~(c / o) * o)),
        e._pt = d = new gt(e._pt,n,i,s,c,Kg),
        d.e = u,
        d.u = "deg",
        e._props.push(i),
        d
    }, Zl = function(e, n) {
        for (var i in n)
            e[i] = n[i];
        return e
    }, ub = function(e, n, i) {
        var s = Zl({}, i._gsap), r = "perspective,force3D,transformOrigin,svgOrigin", o = i.style, a, l, c, u, f, d, _, m;
        s.svg ? (c = i.getAttribute("transform"),
        i.setAttribute("transform", ""),
        o[Oe] = n,
        a = vs(i, 1),
        ci(i, Oe),
        i.setAttribute("transform", c)) : (c = getComputedStyle(i)[Oe],
        o[Oe] = n,
        a = vs(i, 1),
        o[Oe] = c);
        for (l in gn)
            c = s[l],
            u = a[l],
            c !== u && r.indexOf(l) < 0 && (_ = et(c),
            m = et(u),
            f = _ !== m ? Cn(i, l, c, m) : parseFloat(c),
            d = parseFloat(u),
            e._pt = new gt(e._pt,a,l,f,d - f,Mo),
            e._pt.u = m || 0,
            e._props.push(l));
        Zl(a, s)
    };
    mt("padding,margin,Width,Radius", function(t, e) {
        var n = "Top"
          , i = "Right"
          , s = "Bottom"
          , r = "Left"
          , o = (e < 3 ? [n, i, s, r] : [n + r, n + i, s + i, s + r]).map(function(a) {
            return e < 2 ? t + a : "border" + a + t
        });
        cr[e > 1 ? "border" + t : t] = function(a, l, c, u, f) {
            var d, _;
            if (arguments.length < 4)
                return d = o.map(function(m) {
                    return pn(a, m, c)
                }),
                _ = d.join(" "),
                _.split(d[0]).length === 5 ? d[0] : _;
            d = (u + "").split(" "),
            _ = {},
            o.forEach(function(m, p) {
                return _[m] = d[p] = d[p] || d[(p - 1) / 2 | 0]
            }),
            a.init(l, _, f)
        }
    });
    var pd = {
        name: "css",
        register: Go,
        targetTest: function(e) {
            return e.style && e.nodeType
        },
        init: function(e, n, i, s, r) {
            var o = this._props, a = e.style, l = i.vars.startAt, c, u, f, d, _, m, p, b, E, S, v, y, A, T, O, C;
            Ra || Go(),
            this.styles = this.styles || od(e),
            C = this.styles.props,
            this.tween = i;
            for (p in n)
                if (p !== "autoRound" && (u = n[p],
                !(Et[p] && Yf(p, n, i, s, e, r)))) {
                    if (_ = typeof u,
                    m = cr[p],
                    _ === "function" && (u = u.call(i, s, e, r),
                    _ = typeof u),
                    _ === "string" && ~u.indexOf("random(") && (u = ms(u)),
                    m)
                        m(this, e, p, u, i) && (O = 1);
                    else if (p.substr(0, 2) === "--")
                        c = (getComputedStyle(e).getPropertyValue(p) + "").trim(),
                        u += "",
                        Pn.lastIndex = 0,
                        Pn.test(c) || (b = et(c),
                        E = et(u)),
                        E ? b !== E && (c = Cn(e, p, c, E) + E) : b && (u += b),
                        this.add(a, "setProperty", c, u, s, r, 0, 0, p),
                        o.push(p),
                        C.push(p, 0, a[p]);
                    else if (_ !== "undefined") {
                        if (l && p in l ? (c = typeof l[p] == "function" ? l[p].call(i, s, e, r) : l[p],
                        He(c) && ~c.indexOf("random(") && (c = ms(c)),
                        et(c + "") || c === "auto" || (c += xt.units[p] || et(pn(e, p)) || ""),
                        (c + "").charAt(1) === "=" && (c = pn(e, p))) : c = pn(e, p),
                        d = parseFloat(c),
                        S = _ === "string" && u.charAt(1) === "=" && u.substr(0, 2),
                        S && (u = u.substr(2)),
                        f = parseFloat(u),
                        p in Qt && (p === "autoAlpha" && (d === 1 && pn(e, "visibility") === "hidden" && f && (d = 0),
                        C.push("visibility", 0, a.visibility),
                        En(this, a, "visibility", d ? "inherit" : "hidden", f ? "inherit" : "hidden", !f)),
                        p !== "scale" && p !== "transform" && (p = Qt[p],
                        ~p.indexOf(",") && (p = p.split(",")[0]))),
                        v = p in gn,
                        v) {
                            if (this.styles.save(p),
                            y || (A = e._gsap,
                            A.renderTransform && !n.parseTransform || vs(e, n.parseTransform),
                            T = n.smoothOrigin !== !1 && A.smooth,
                            y = this._pt = new gt(this._pt,a,Oe,0,1,A.renderTransform,A,0,-1),
                            y.dep = 1),
                            p === "scale")
                                this._pt = new gt(this._pt,A,"scaleY",A.scaleY,(S ? Ei(A.scaleY, S + f) : f) - A.scaleY || 0,Mo),
                                this._pt.u = 0,
                                o.push("scaleY", p),
                                p += "X";
                            else if (p === "transformOrigin") {
                                C.push(bt, 0, a[bt]),
                                u = rb(u),
                                A.svg ? Uo(e, u, 0, T, 0, this) : (E = parseFloat(u.split(" ")[2]) || 0,
                                E !== A.zOrigin && En(this, A, "zOrigin", A.zOrigin, E),
                                En(this, a, p, ur(c), ur(u)));
                                continue
                            } else if (p === "svgOrigin") {
                                Uo(e, u, 1, T, 0, this);
                                continue
                            } else if (p in ud) {
                                cb(this, A, p, d, S ? Ei(d, S + u) : u);
                                continue
                            } else if (p === "smoothOrigin") {
                                En(this, A, "smooth", A.smooth, u);
                                continue
                            } else if (p === "force3D") {
                                A[p] = u;
                                continue
                            } else if (p === "transform") {
                                ub(this, u, e);
                                continue
                            }
                        } else
                            p in a || (p = Li(p) || p);
                        if (v || (f || f === 0) && (d || d === 0) && !Hg.test(u) && p in a)
                            b = (c + "").substr((d + "").length),
                            f || (f = 0),
                            E = et(u) || (p in xt.units ? xt.units[p] : b),
                            b !== E && (d = Cn(e, p, c, E)),
                            this._pt = new gt(this._pt,v ? A : a,p,d,(S ? Ei(d, S + f) : f) - d,!v && (E === "px" || p === "zIndex") && n.autoRound !== !1 ? Wg : Mo),
                            this._pt.u = E || 0,
                            b !== E && E !== "%" && (this._pt.b = c,
                            this._pt.r = qg);
                        else if (p in a)
                            sb.call(this, e, p, c, S ? S + u : u);
                        else if (p in e)
                            this.add(e, p, c || e[p], S ? S + u : u, s, r);
                        else if (p !== "parseTransform") {
                            Ta(p, u);
                            continue
                        }
                        v || (p in a ? C.push(p, 0, a[p]) : typeof e[p] == "function" ? C.push(p, 2, e[p]()) : C.push(p, 1, c || e[p])),
                        o.push(p)
                    }
                }
            O && td(this)
        },
        render: function(e, n) {
            if (n.tween._time || !La())
                for (var i = n._pt; i; )
                    i.r(e, i.d),
                    i = i._next;
            else
                n.styles.revert()
        },
        get: pn,
        aliases: Qt,
        getSetter: function(e, n, i) {
            var s = Qt[n];
            return s && s.indexOf(",") < 0 && (n = s),
            n in gn && n !== bt && (e._gsap.x || pn(e, "x")) ? i && Hl === i ? n === "scale" ? Qg : Jg : (Hl = i || {}) && (n === "scale" ? Zg : eb) : e.style && !ka(e.style[n]) ? Yg : ~n.indexOf("-") ? Xg : Ia(e, n)
        },
        core: {
            _removeProperty: ci,
            _getMatrix: Da
        }
    };
    yt.utils.checkPrefix = Li;
    yt.core.getStyleSaver = od;
    (function(t, e, n, i) {
        var s = mt(t + "," + e + "," + n, function(r) {
            gn[r] = 1
        });
        mt(e, function(r) {
            xt.units[r] = "deg",
            ud[r] = 1
        }),
        Qt[s[13]] = t + "," + e,
        mt(i, function(r) {
            var o = r.split(":");
            Qt[o[1]] = s[o[0]]
        })
    }
    )("x,y,z,scale,scaleX,scaleY,xPercent,yPercent", "rotation,rotationX,rotationY,skewX,skewY", "transform,transformOrigin,svgOrigin,force3D,smoothOrigin,transformPerspective", "0:translateX,1:translateY,2:translateZ,8:rotate,8:rotationZ,8:rotateZ,9:rotateX,10:rotateY");
    mt("x,y,z,top,right,bottom,left,width,height,fontSize,padding,margin,perspective", function(t) {
        xt.units[t] = "px"
    });
    yt.registerPlugin(pd);
    var qs = yt.registerPlugin(pd) || yt;
    qs.core.Tween;
    const fb = {
        version: "1.0.7.6"
    };
    /*!
  * shared v9.14.2
  * (c) 2024 kazuya kawaguchi
  * Released under the MIT License.
  */
    const fr = typeof window != "undefined"
      , Fn = (t, e=!1) => e ? Symbol.for(t) : Symbol(t)
      , db = (t, e, n) => pb({
        l: t,
        k: e,
        s: n
    })
      , pb = t => JSON.stringify(t).replace(/\u2028/g, "\\u2028").replace(/\u2029/g, "\\u2029").replace(/\u0027/g, "\\u0027")
      , De = t => typeof t == "number" && isFinite(t)
      , _b = t => hd(t) === "[object Date]"
      , Rn = t => hd(t) === "[object RegExp]"
      , Pr = t => ne(t) && Object.keys(t).length === 0
      , Ye = Object.assign
      , hb = Object.create
      , ye = (t=null) => hb(t);
    let ec;
    const hn = () => ec || (ec = typeof globalThis != "undefined" ? globalThis : typeof self != "undefined" ? self : typeof window != "undefined" ? window : typeof global != "undefined" ? global : ye());
    function tc(t) {
        return t.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&apos;")
    }
    const mb = Object.prototype.hasOwnProperty;
    function Ut(t, e) {
        return mb.call(t, e)
    }
    const Te = Array.isArray
      , $e = t => typeof t == "function"
      , H = t => typeof t == "string"
      , oe = t => typeof t == "boolean"
      , fe = t => t !== null && typeof t == "object"
      , gb = t => fe(t) && $e(t.then) && $e(t.catch)
      , _d = Object.prototype.toString
      , hd = t => _d.call(t)
      , ne = t => {
        if (!fe(t))
            return !1;
        const e = Object.getPrototypeOf(t);
        return e === null || e.constructor === Object
    }
      , bb = t => t == null ? "" : Te(t) || ne(t) && t.toString === _d ? JSON.stringify(t, null, 2) : String(t);
    function yb(t, e="") {
        return t.reduce( (n, i, s) => s === 0 ? n + i : n + e + i, "")
    }
    function xr(t) {
        let e = t;
        return () => ++e
    }
    function vb(t, e) {
        typeof console != "undefined" && (console.warn("[intlify] " + t),
        e && console.warn(e.stack))
    }
    const Ds = t => !fe(t) || Te(t);
    function Ws(t, e) {
        if (Ds(t) || Ds(e))
            throw new Error("Invalid value");
        const n = [{
            src: t,
            des: e
        }];
        for (; n.length; ) {
            const {src: i, des: s} = n.pop();
            Object.keys(i).forEach(r => {
                r !== "__proto__" && (fe(i[r]) && !fe(s[r]) && (s[r] = Array.isArray(i[r]) ? [] : ye()),
                Ds(s[r]) || Ds(i[r]) ? s[r] = i[r] : n.push({
                    src: i[r],
                    des: s[r]
                }))
            }
            )
        }
    }
    /*!
  * message-compiler v9.14.2
  * (c) 2024 kazuya kawaguchi
  * Released under the MIT License.
  */
    function kb(t, e, n) {
        return {
            line: t,
            column: e,
            offset: n
        }
    }
    function dr(t, e, n) {
        return {
            start: t,
            end: e
        }
    }
    const Ab = /\{([0-9a-zA-Z]+)\}/g;
    function md(t, ...e) {
        return e.length === 1 && $b(e[0]) && (e = e[0]),
        (!e || !e.hasOwnProperty) && (e = {}),
        t.replace(Ab, (n, i) => e.hasOwnProperty(i) ? e[i] : "")
    }
    const gd = Object.assign
      , nc = t => typeof t == "string"
      , $b = t => t !== null && typeof t == "object";
    function bd(t, e="") {
        return t.reduce( (n, i, s) => s === 0 ? n + i : n + e + i, "")
    }
    const Ma = {
        USE_MODULO_SYNTAX: 1,
        __EXTEND_POINT__: 2
    }
      , Tb = {
        [Ma.USE_MODULO_SYNTAX]: "Use modulo before '{{0}}'."
    };
    function Eb(t, e, ...n) {
        const i = md(Tb[t], ...n || [])
          , s = {
            message: String(i),
            code: t
        };
        return e && (s.location = e),
        s
    }
    const ee = {
        EXPECTED_TOKEN: 1,
        INVALID_TOKEN_IN_PLACEHOLDER: 2,
        UNTERMINATED_SINGLE_QUOTE_IN_PLACEHOLDER: 3,
        UNKNOWN_ESCAPE_SEQUENCE: 4,
        INVALID_UNICODE_ESCAPE_SEQUENCE: 5,
        UNBALANCED_CLOSING_BRACE: 6,
        UNTERMINATED_CLOSING_BRACE: 7,
        EMPTY_PLACEHOLDER: 8,
        NOT_ALLOW_NEST_PLACEHOLDER: 9,
        INVALID_LINKED_FORMAT: 10,
        MUST_HAVE_MESSAGES_IN_PLURAL: 11,
        UNEXPECTED_EMPTY_LINKED_MODIFIER: 12,
        UNEXPECTED_EMPTY_LINKED_KEY: 13,
        UNEXPECTED_LEXICAL_ANALYSIS: 14,
        UNHANDLED_CODEGEN_NODE_TYPE: 15,
        UNHANDLED_MINIFIER_NODE_TYPE: 16,
        __EXTEND_POINT__: 17
    }
      , wb = {
        [ee.EXPECTED_TOKEN]: "Expected token: '{0}'",
        [ee.INVALID_TOKEN_IN_PLACEHOLDER]: "Invalid token in placeholder: '{0}'",
        [ee.UNTERMINATED_SINGLE_QUOTE_IN_PLACEHOLDER]: "Unterminated single quote in placeholder",
        [ee.UNKNOWN_ESCAPE_SEQUENCE]: "Unknown escape sequence: \\{0}",
        [ee.INVALID_UNICODE_ESCAPE_SEQUENCE]: "Invalid unicode escape sequence: {0}",
        [ee.UNBALANCED_CLOSING_BRACE]: "Unbalanced closing brace",
        [ee.UNTERMINATED_CLOSING_BRACE]: "Unterminated closing brace",
        [ee.EMPTY_PLACEHOLDER]: "Empty placeholder",
        [ee.NOT_ALLOW_NEST_PLACEHOLDER]: "Not allowed nest placeholder",
        [ee.INVALID_LINKED_FORMAT]: "Invalid linked format",
        [ee.MUST_HAVE_MESSAGES_IN_PLURAL]: "Plural must have messages",
        [ee.UNEXPECTED_EMPTY_LINKED_MODIFIER]: "Unexpected empty linked modifier",
        [ee.UNEXPECTED_EMPTY_LINKED_KEY]: "Unexpected empty linked key",
        [ee.UNEXPECTED_LEXICAL_ANALYSIS]: "Unexpected lexical analysis in token: '{0}'",
        [ee.UNHANDLED_CODEGEN_NODE_TYPE]: "unhandled codegen node type: '{0}'",
        [ee.UNHANDLED_MINIFIER_NODE_TYPE]: "unhandled mimifier node type: '{0}'"
    };
    function Ui(t, e, n={}) {
        const {domain: i, messages: s, args: r} = n
          , o = md((s || wb)[t] || "", ...r || [])
          , a = new SyntaxError(String(o));
        return a.code = t,
        e && (a.location = e),
        a.domain = i,
        a
    }
    function Sb(t) {
        throw t
    }
    const cn = " "
      , Ob = "\r"
      , at = "\n"
      , Pb = "\u2028"
      , xb = "\u2029";
    function Ib(t) {
        const e = t;
        let n = 0
          , i = 1
          , s = 1
          , r = 0;
        const o = O => e[O] === Ob && e[O + 1] === at
          , a = O => e[O] === at
          , l = O => e[O] === xb
          , c = O => e[O] === Pb
          , u = O => o(O) || a(O) || l(O) || c(O)
          , f = () => n
          , d = () => i
          , _ = () => s
          , m = () => r
          , p = O => o(O) || l(O) || c(O) ? at : e[O]
          , b = () => p(n)
          , E = () => p(n + r);
        function S() {
            return r = 0,
            u(n) && (i++,
            s = 0),
            o(n) && n++,
            n++,
            s++,
            e[n]
        }
        function v() {
            return o(n + r) && r++,
            r++,
            e[n + r]
        }
        function y() {
            n = 0,
            i = 1,
            s = 1,
            r = 0
        }
        function A(O=0) {
            r = O
        }
        function T() {
            const O = n + r;
            for (; O !== n; )
                S();
            r = 0
        }
        return {
            index: f,
            line: d,
            column: _,
            peekOffset: m,
            charAt: p,
            currentChar: b,
            currentPeek: E,
            next: S,
            peek: v,
            reset: y,
            resetPeek: A,
            skipToPeek: T
        }
    }
    const yn = void 0
      , Cb = "."
      , ic = "'"
      , Rb = "tokenizer";
    function Lb(t, e={}) {
        const n = e.location !== !1
          , i = Ib(t)
          , s = () => i.index()
          , r = () => kb(i.line(), i.column(), i.index())
          , o = r()
          , a = s()
          , l = {
            currentType: 14,
            offset: a,
            startLoc: o,
            endLoc: o,
            lastType: 14,
            lastOffset: a,
            lastStartLoc: o,
            lastEndLoc: o,
            braceNest: 0,
            inLinked: !1,
            text: ""
        }
          , c = () => l
          , {onError: u} = e;
        function f(h, g, I, ...L) {
            const V = c();
            if (g.column += I,
            g.offset += I,
            u) {
                const U = n ? dr(V.startLoc, g) : null
                  , x = Ui(h, U, {
                    domain: Rb,
                    args: L
                });
                u(x)
            }
        }
        function d(h, g, I) {
            h.endLoc = r(),
            h.currentType = g;
            const L = {
                type: g
            };
            return n && (L.loc = dr(h.startLoc, h.endLoc)),
            I != null && (L.value = I),
            L
        }
        const _ = h => d(h, 14);
        function m(h, g) {
            return h.currentChar() === g ? (h.next(),
            g) : (f(ee.EXPECTED_TOKEN, r(), 0, g),
            "")
        }
        function p(h) {
            let g = "";
            for (; h.currentPeek() === cn || h.currentPeek() === at; )
                g += h.currentPeek(),
                h.peek();
            return g
        }
        function b(h) {
            const g = p(h);
            return h.skipToPeek(),
            g
        }
        function E(h) {
            if (h === yn)
                return !1;
            const g = h.charCodeAt(0);
            return g >= 97 && g <= 122 || g >= 65 && g <= 90 || g === 95
        }
        function S(h) {
            if (h === yn)
                return !1;
            const g = h.charCodeAt(0);
            return g >= 48 && g <= 57
        }
        function v(h, g) {
            const {currentType: I} = g;
            if (I !== 2)
                return !1;
            p(h);
            const L = E(h.currentPeek());
            return h.resetPeek(),
            L
        }
        function y(h, g) {
            const {currentType: I} = g;
            if (I !== 2)
                return !1;
            p(h);
            const L = h.currentPeek() === "-" ? h.peek() : h.currentPeek()
              , V = S(L);
            return h.resetPeek(),
            V
        }
        function A(h, g) {
            const {currentType: I} = g;
            if (I !== 2)
                return !1;
            p(h);
            const L = h.currentPeek() === ic;
            return h.resetPeek(),
            L
        }
        function T(h, g) {
            const {currentType: I} = g;
            if (I !== 8)
                return !1;
            p(h);
            const L = h.currentPeek() === ".";
            return h.resetPeek(),
            L
        }
        function O(h, g) {
            const {currentType: I} = g;
            if (I !== 9)
                return !1;
            p(h);
            const L = E(h.currentPeek());
            return h.resetPeek(),
            L
        }
        function C(h, g) {
            const {currentType: I} = g;
            if (!(I === 8 || I === 12))
                return !1;
            p(h);
            const L = h.currentPeek() === ":";
            return h.resetPeek(),
            L
        }
        function P(h, g) {
            const {currentType: I} = g;
            if (I !== 10)
                return !1;
            const L = () => {
                const U = h.currentPeek();
                return U === "{" ? E(h.peek()) : U === "@" || U === "%" || U === "|" || U === ":" || U === "." || U === cn || !U ? !1 : U === at ? (h.peek(),
                L()) : q(h, !1)
            }
              , V = L();
            return h.resetPeek(),
            V
        }
        function W(h) {
            p(h);
            const g = h.currentPeek() === "|";
            return h.resetPeek(),
            g
        }
        function X(h) {
            const g = p(h)
              , I = h.currentPeek() === "%" && h.peek() === "{";
            return h.resetPeek(),
            {
                isModulo: I,
                hasSpace: g.length > 0
            }
        }
        function q(h, g=!0) {
            const I = (V=!1, U="", x=!1) => {
                const D = h.currentPeek();
                return D === "{" ? U === "%" ? !1 : V : D === "@" || !D ? U === "%" ? !0 : V : D === "%" ? (h.peek(),
                I(V, "%", !0)) : D === "|" ? U === "%" || x ? !0 : !(U === cn || U === at) : D === cn ? (h.peek(),
                I(!0, cn, x)) : D === at ? (h.peek(),
                I(!0, at, x)) : !0
            }
              , L = I();
            return g && h.resetPeek(),
            L
        }
        function Q(h, g) {
            const I = h.currentChar();
            return I === yn ? yn : g(I) ? (h.next(),
            I) : null
        }
        function _e(h) {
            const g = h.charCodeAt(0);
            return g >= 97 && g <= 122 || g >= 65 && g <= 90 || g >= 48 && g <= 57 || g === 95 || g === 36
        }
        function ue(h) {
            return Q(h, _e)
        }
        function Z(h) {
            const g = h.charCodeAt(0);
            return g >= 97 && g <= 122 || g >= 65 && g <= 90 || g >= 48 && g <= 57 || g === 95 || g === 36 || g === 45
        }
        function J(h) {
            return Q(h, Z)
        }
        function te(h) {
            const g = h.charCodeAt(0);
            return g >= 48 && g <= 57
        }
        function Ie(h) {
            return Q(h, te)
        }
        function ze(h) {
            const g = h.charCodeAt(0);
            return g >= 48 && g <= 57 || g >= 65 && g <= 70 || g >= 97 && g <= 102
        }
        function he(h) {
            return Q(h, ze)
        }
        function me(h) {
            let g = ""
              , I = "";
            for (; g = Ie(h); )
                I += g;
            return I
        }
        function vt(h) {
            b(h);
            const g = h.currentChar();
            return g !== "%" && f(ee.EXPECTED_TOKEN, r(), 0, g),
            h.next(),
            "%"
        }
        function G(h) {
            let g = "";
            for (; ; ) {
                const I = h.currentChar();
                if (I === "{" || I === "}" || I === "@" || I === "|" || !I)
                    break;
                if (I === "%")
                    if (q(h))
                        g += I,
                        h.next();
                    else
                        break;
                else if (I === cn || I === at)
                    if (q(h))
                        g += I,
                        h.next();
                    else {
                        if (W(h))
                            break;
                        g += I,
                        h.next()
                    }
                else
                    g += I,
                    h.next()
            }
            return g
        }
        function K(h) {
            b(h);
            let g = ""
              , I = "";
            for (; g = J(h); )
                I += g;
            return h.currentChar() === yn && f(ee.UNTERMINATED_CLOSING_BRACE, r(), 0),
            I
        }
        function ge(h) {
            b(h);
            let g = "";
            return h.currentChar() === "-" ? (h.next(),
            g += "-".concat(me(h))) : g += me(h),
            h.currentChar() === yn && f(ee.UNTERMINATED_CLOSING_BRACE, r(), 0),
            g
        }
        function be(h) {
            return h !== ic && h !== at
        }
        function Ue(h) {
            b(h),
            m(h, "'");
            let g = ""
              , I = "";
            for (; g = Q(h, be); )
                g === "\\" ? I += Ce(h) : I += g;
            const L = h.currentChar();
            return L === at || L === yn ? (f(ee.UNTERMINATED_SINGLE_QUOTE_IN_PLACEHOLDER, r(), 0),
            L === at && (h.next(),
            m(h, "'")),
            I) : (m(h, "'"),
            I)
        }
        function Ce(h) {
            const g = h.currentChar();
            switch (g) {
            case "\\":
            case "'":
                return h.next(),
                "\\".concat(g);
            case "u":
                return Re(h, g, 4);
            case "U":
                return Re(h, g, 6);
            default:
                return f(ee.UNKNOWN_ESCAPE_SEQUENCE, r(), 0, g),
                ""
            }
        }
        function Re(h, g, I) {
            m(h, g);
            let L = "";
            for (let V = 0; V < I; V++) {
                const U = he(h);
                if (!U) {
                    f(ee.INVALID_UNICODE_ESCAPE_SEQUENCE, r(), 0, "\\".concat(g).concat(L).concat(h.currentChar()));
                    break
                }
                L += U
            }
            return "\\".concat(g).concat(L)
        }
        function on(h) {
            return h !== "{" && h !== "}" && h !== cn && h !== at
        }
        function k(h) {
            b(h);
            let g = ""
              , I = "";
            for (; g = Q(h, on); )
                I += g;
            return I
        }
        function w(h) {
            let g = ""
              , I = "";
            for (; g = ue(h); )
                I += g;
            return I
        }
        function $(h) {
            const g = I => {
                const L = h.currentChar();
                return L === "{" || L === "%" || L === "@" || L === "|" || L === "(" || L === ")" || !L || L === cn ? I : (I += L,
                h.next(),
                g(I))
            }
            ;
            return g("")
        }
        function N(h) {
            b(h);
            const g = m(h, "|");
            return b(h),
            g
        }
        function M(h, g) {
            let I = null;
            switch (h.currentChar()) {
            case "{":
                return g.braceNest >= 1 && f(ee.NOT_ALLOW_NEST_PLACEHOLDER, r(), 0),
                h.next(),
                I = d(g, 2, "{"),
                b(h),
                g.braceNest++,
                I;
            case "}":
                return g.braceNest > 0 && g.currentType === 2 && f(ee.EMPTY_PLACEHOLDER, r(), 0),
                h.next(),
                I = d(g, 3, "}"),
                g.braceNest--,
                g.braceNest > 0 && b(h),
                g.inLinked && g.braceNest === 0 && (g.inLinked = !1),
                I;
            case "@":
                return g.braceNest > 0 && f(ee.UNTERMINATED_CLOSING_BRACE, r(), 0),
                I = F(h, g) || _(g),
                g.braceNest = 0,
                I;
            default:
                {
                    let V = !0
                      , U = !0
                      , x = !0;
                    if (W(h))
                        return g.braceNest > 0 && f(ee.UNTERMINATED_CLOSING_BRACE, r(), 0),
                        I = d(g, 1, N(h)),
                        g.braceNest = 0,
                        g.inLinked = !1,
                        I;
                    if (g.braceNest > 0 && (g.currentType === 5 || g.currentType === 6 || g.currentType === 7))
                        return f(ee.UNTERMINATED_CLOSING_BRACE, r(), 0),
                        g.braceNest = 0,
                        z(h, g);
                    if (V = v(h, g))
                        return I = d(g, 5, K(h)),
                        b(h),
                        I;
                    if (U = y(h, g))
                        return I = d(g, 6, ge(h)),
                        b(h),
                        I;
                    if (x = A(h, g))
                        return I = d(g, 7, Ue(h)),
                        b(h),
                        I;
                    if (!V && !U && !x)
                        return I = d(g, 13, k(h)),
                        f(ee.INVALID_TOKEN_IN_PLACEHOLDER, r(), 0, I.value),
                        b(h),
                        I;
                    break
                }
            }
            return I
        }
        function F(h, g) {
            const {currentType: I} = g;
            let L = null;
            const V = h.currentChar();
            switch ((I === 8 || I === 9 || I === 12 || I === 10) && (V === at || V === cn) && f(ee.INVALID_LINKED_FORMAT, r(), 0),
            V) {
            case "@":
                return h.next(),
                L = d(g, 8, "@"),
                g.inLinked = !0,
                L;
            case ".":
                return b(h),
                h.next(),
                d(g, 9, ".");
            case ":":
                return b(h),
                h.next(),
                d(g, 10, ":");
            default:
                return W(h) ? (L = d(g, 1, N(h)),
                g.braceNest = 0,
                g.inLinked = !1,
                L) : T(h, g) || C(h, g) ? (b(h),
                F(h, g)) : O(h, g) ? (b(h),
                d(g, 12, w(h))) : P(h, g) ? (b(h),
                V === "{" ? M(h, g) || L : d(g, 11, $(h))) : (I === 8 && f(ee.INVALID_LINKED_FORMAT, r(), 0),
                g.braceNest = 0,
                g.inLinked = !1,
                z(h, g))
            }
        }
        function z(h, g) {
            let I = {
                type: 14
            };
            if (g.braceNest > 0)
                return M(h, g) || _(g);
            if (g.inLinked)
                return F(h, g) || _(g);
            switch (h.currentChar()) {
            case "{":
                return M(h, g) || _(g);
            case "}":
                return f(ee.UNBALANCED_CLOSING_BRACE, r(), 0),
                h.next(),
                d(g, 3, "}");
            case "@":
                return F(h, g) || _(g);
            default:
                {
                    if (W(h))
                        return I = d(g, 1, N(h)),
                        g.braceNest = 0,
                        g.inLinked = !1,
                        I;
                    const {isModulo: V, hasSpace: U} = X(h);
                    if (V)
                        return U ? d(g, 0, G(h)) : d(g, 4, vt(h));
                    if (q(h))
                        return d(g, 0, G(h));
                    break
                }
            }
            return I
        }
        function B() {
            const {currentType: h, offset: g, startLoc: I, endLoc: L} = l;
            return l.lastType = h,
            l.lastOffset = g,
            l.lastStartLoc = I,
            l.lastEndLoc = L,
            l.offset = s(),
            l.startLoc = r(),
            i.currentChar() === yn ? d(l, 14) : z(i, l)
        }
        return {
            nextToken: B,
            currentOffset: s,
            currentPosition: r,
            context: c
        }
    }
    const Nb = "parser"
      , Db = /(?:\\\\|\\'|\\u([0-9a-fA-F]{4})|\\U([0-9a-fA-F]{6}))/g;
    function Mb(t, e, n) {
        switch (t) {
        case "\\\\":
            return "\\";
        case "\\'":
            return "'";
        default:
            {
                const i = parseInt(e || n, 16);
                return i <= 55295 || i >= 57344 ? String.fromCodePoint(i) : "�"
            }
        }
    }
    function Fb(t={}) {
        const e = t.location !== !1
          , {onError: n, onWarn: i} = t;
        function s(v, y, A, T, ...O) {
            const C = v.currentPosition();
            if (C.offset += T,
            C.column += T,
            n) {
                const P = e ? dr(A, C) : null
                  , W = Ui(y, P, {
                    domain: Nb,
                    args: O
                });
                n(W)
            }
        }
        function r(v, y, A, T, ...O) {
            const C = v.currentPosition();
            if (C.offset += T,
            C.column += T,
            i) {
                const P = e ? dr(A, C) : null;
                i(Eb(y, P, O))
            }
        }
        function o(v, y, A) {
            const T = {
                type: v
            };
            return e && (T.start = y,
            T.end = y,
            T.loc = {
                start: A,
                end: A
            }),
            T
        }
        function a(v, y, A, T) {
            e && (v.end = y,
            v.loc && (v.loc.end = A))
        }
        function l(v, y) {
            const A = v.context()
              , T = o(3, A.offset, A.startLoc);
            return T.value = y,
            a(T, v.currentOffset(), v.currentPosition()),
            T
        }
        function c(v, y) {
            const A = v.context()
              , {lastOffset: T, lastStartLoc: O} = A
              , C = o(5, T, O);
            return C.index = parseInt(y, 10),
            v.nextToken(),
            a(C, v.currentOffset(), v.currentPosition()),
            C
        }
        function u(v, y, A) {
            const T = v.context()
              , {lastOffset: O, lastStartLoc: C} = T
              , P = o(4, O, C);
            return P.key = y,
            A === !0 && (P.modulo = !0),
            v.nextToken(),
            a(P, v.currentOffset(), v.currentPosition()),
            P
        }
        function f(v, y) {
            const A = v.context()
              , {lastOffset: T, lastStartLoc: O} = A
              , C = o(9, T, O);
            return C.value = y.replace(Db, Mb),
            v.nextToken(),
            a(C, v.currentOffset(), v.currentPosition()),
            C
        }
        function d(v) {
            const y = v.nextToken()
              , A = v.context()
              , {lastOffset: T, lastStartLoc: O} = A
              , C = o(8, T, O);
            return y.type !== 12 ? (s(v, ee.UNEXPECTED_EMPTY_LINKED_MODIFIER, A.lastStartLoc, 0),
            C.value = "",
            a(C, T, O),
            {
                nextConsumeToken: y,
                node: C
            }) : (y.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, A.lastStartLoc, 0, Dt(y)),
            C.value = y.value || "",
            a(C, v.currentOffset(), v.currentPosition()),
            {
                node: C
            })
        }
        function _(v, y) {
            const A = v.context()
              , T = o(7, A.offset, A.startLoc);
            return T.value = y,
            a(T, v.currentOffset(), v.currentPosition()),
            T
        }
        function m(v) {
            const y = v.context()
              , A = o(6, y.offset, y.startLoc);
            let T = v.nextToken();
            if (T.type === 9) {
                const O = d(v);
                A.modifier = O.node,
                T = O.nextConsumeToken || v.nextToken()
            }
            switch (T.type !== 10 && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(T)),
            T = v.nextToken(),
            T.type === 2 && (T = v.nextToken()),
            T.type) {
            case 11:
                T.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(T)),
                A.key = _(v, T.value || "");
                break;
            case 5:
                T.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(T)),
                A.key = u(v, T.value || "");
                break;
            case 6:
                T.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(T)),
                A.key = c(v, T.value || "");
                break;
            case 7:
                T.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(T)),
                A.key = f(v, T.value || "");
                break;
            default:
                {
                    s(v, ee.UNEXPECTED_EMPTY_LINKED_KEY, y.lastStartLoc, 0);
                    const O = v.context()
                      , C = o(7, O.offset, O.startLoc);
                    return C.value = "",
                    a(C, O.offset, O.startLoc),
                    A.key = C,
                    a(A, O.offset, O.startLoc),
                    {
                        nextConsumeToken: T,
                        node: A
                    }
                }
            }
            return a(A, v.currentOffset(), v.currentPosition()),
            {
                node: A
            }
        }
        function p(v) {
            const y = v.context()
              , A = y.currentType === 1 ? v.currentOffset() : y.offset
              , T = y.currentType === 1 ? y.endLoc : y.startLoc
              , O = o(2, A, T);
            O.items = [];
            let C = null
              , P = null;
            do {
                const q = C || v.nextToken();
                switch (C = null,
                q.type) {
                case 0:
                    q.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(q)),
                    O.items.push(l(v, q.value || ""));
                    break;
                case 6:
                    q.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(q)),
                    O.items.push(c(v, q.value || ""));
                    break;
                case 4:
                    P = !0;
                    break;
                case 5:
                    q.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(q)),
                    O.items.push(u(v, q.value || "", !!P)),
                    P && (r(v, Ma.USE_MODULO_SYNTAX, y.lastStartLoc, 0, Dt(q)),
                    P = null);
                    break;
                case 7:
                    q.value == null && s(v, ee.UNEXPECTED_LEXICAL_ANALYSIS, y.lastStartLoc, 0, Dt(q)),
                    O.items.push(f(v, q.value || ""));
                    break;
                case 8:
                    {
                        const Q = m(v);
                        O.items.push(Q.node),
                        C = Q.nextConsumeToken || null;
                        break
                    }
                }
            } while (y.currentType !== 14 && y.currentType !== 1);
            const W = y.currentType === 1 ? y.lastOffset : v.currentOffset()
              , X = y.currentType === 1 ? y.lastEndLoc : v.currentPosition();
            return a(O, W, X),
            O
        }
        function b(v, y, A, T) {
            const O = v.context();
            let C = T.items.length === 0;
            const P = o(1, y, A);
            P.cases = [],
            P.cases.push(T);
            do {
                const W = p(v);
                C || (C = W.items.length === 0),
                P.cases.push(W)
            } while (O.currentType !== 14);
            return C && s(v, ee.MUST_HAVE_MESSAGES_IN_PLURAL, A, 0),
            a(P, v.currentOffset(), v.currentPosition()),
            P
        }
        function E(v) {
            const y = v.context()
              , {offset: A, startLoc: T} = y
              , O = p(v);
            return y.currentType === 14 ? O : b(v, A, T, O)
        }
        function S(v) {
            const y = Lb(v, gd({}, t))
              , A = y.context()
              , T = o(0, A.offset, A.startLoc);
            return e && T.loc && (T.loc.source = v),
            T.body = E(y),
            t.onCacheKey && (T.cacheKey = t.onCacheKey(v)),
            A.currentType !== 14 && s(y, ee.UNEXPECTED_LEXICAL_ANALYSIS, A.lastStartLoc, 0, v[A.offset] || ""),
            a(T, y.currentOffset(), y.currentPosition()),
            T
        }
        return {
            parse: S
        }
    }
    function Dt(t) {
        if (t.type === 14)
            return "EOF";
        const e = (t.value || "").replace(/\r?\n/gu, "\\n");
        return e.length > 10 ? e.slice(0, 9) + "…" : e
    }
    function Gb(t, e={}) {
        const n = {
            ast: t,
            helpers: new Set
        };
        return {
            context: () => n,
            helper: r => (n.helpers.add(r),
            r)
        }
    }
    function sc(t, e) {
        for (let n = 0; n < t.length; n++)
            Fa(t[n], e)
    }
    function Fa(t, e) {
        switch (t.type) {
        case 1:
            sc(t.cases, e),
            e.helper("plural");
            break;
        case 2:
            sc(t.items, e);
            break;
        case 6:
            {
                Fa(t.key, e),
                e.helper("linked"),
                e.helper("type");
                break
            }
        case 5:
            e.helper("interpolate"),
            e.helper("list");
            break;
        case 4:
            e.helper("interpolate"),
            e.helper("named");
            break
        }
    }
    function Ub(t, e={}) {
        const n = Gb(t);
        n.helper("normalize"),
        t.body && Fa(t.body, n);
        const i = n.context();
        t.helpers = Array.from(i.helpers)
    }
    function jb(t) {
        const e = t.body;
        return e.type === 2 ? rc(e) : e.cases.forEach(n => rc(n)),
        t
    }
    function rc(t) {
        if (t.items.length === 1) {
            const e = t.items[0];
            (e.type === 3 || e.type === 9) && (t.static = e.value,
            delete e.value)
        } else {
            const e = [];
            for (let n = 0; n < t.items.length; n++) {
                const i = t.items[n];
                if (!(i.type === 3 || i.type === 9) || i.value == null)
                    break;
                e.push(i.value)
            }
            if (e.length === t.items.length) {
                t.static = bd(e);
                for (let n = 0; n < t.items.length; n++) {
                    const i = t.items[n];
                    (i.type === 3 || i.type === 9) && delete i.value
                }
            }
        }
    }
    const Bb = "minifier";
    function gi(t) {
        switch (t.t = t.type,
        t.type) {
        case 0:
            {
                const e = t;
                gi(e.body),
                e.b = e.body,
                delete e.body;
                break
            }
        case 1:
            {
                const e = t
                  , n = e.cases;
                for (let i = 0; i < n.length; i++)
                    gi(n[i]);
                e.c = n,
                delete e.cases;
                break
            }
        case 2:
            {
                const e = t
                  , n = e.items;
                for (let i = 0; i < n.length; i++)
                    gi(n[i]);
                e.i = n,
                delete e.items,
                e.static && (e.s = e.static,
                delete e.static);
                break
            }
        case 3:
        case 9:
        case 8:
        case 7:
            {
                const e = t;
                e.value && (e.v = e.value,
                delete e.value);
                break
            }
        case 6:
            {
                const e = t;
                gi(e.key),
                e.k = e.key,
                delete e.key,
                e.modifier && (gi(e.modifier),
                e.m = e.modifier,
                delete e.modifier);
                break
            }
        case 5:
            {
                const e = t;
                e.i = e.index,
                delete e.index;
                break
            }
        case 4:
            {
                const e = t;
                e.k = e.key,
                delete e.key;
                break
            }
        default:
            throw Ui(ee.UNHANDLED_MINIFIER_NODE_TYPE, null, {
                domain: Bb,
                args: [t.type]
            })
        }
        delete t.type
    }
    const zb = "parser";
    function Vb(t, e) {
        const {filename: n, breakLineCode: i, needIndent: s} = e
          , r = e.location !== !1
          , o = {
            filename: n,
            code: "",
            column: 1,
            line: 1,
            offset: 0,
            map: void 0,
            breakLineCode: i,
            needIndent: s,
            indentLevel: 0
        };
        r && t.loc && (o.source = t.loc.source);
        const a = () => o;
        function l(p, b) {
            o.code += p
        }
        function c(p, b=!0) {
            const E = b ? i : "";
            l(s ? E + "  ".repeat(p) : E)
        }
        function u(p=!0) {
            const b = ++o.indentLevel;
            p && c(b)
        }
        function f(p=!0) {
            const b = --o.indentLevel;
            p && c(b)
        }
        function d() {
            c(o.indentLevel)
        }
        return {
            context: a,
            push: l,
            indent: u,
            deindent: f,
            newline: d,
            helper: p => "_".concat(p),
            needIndent: () => o.needIndent
        }
    }
    function Hb(t, e) {
        const {helper: n} = t;
        t.push("".concat(n("linked"), "(")),
        Ni(t, e.key),
        e.modifier ? (t.push(", "),
        Ni(t, e.modifier),
        t.push(", _type")) : t.push(", undefined, _type"),
        t.push(")")
    }
    function Kb(t, e) {
        const {helper: n, needIndent: i} = t;
        t.push("".concat(n("normalize"), "([")),
        t.indent(i());
        const s = e.items.length;
        for (let r = 0; r < s && (Ni(t, e.items[r]),
        r !== s - 1); r++)
            t.push(", ");
        t.deindent(i()),
        t.push("])")
    }
    function qb(t, e) {
        const {helper: n, needIndent: i} = t;
        if (e.cases.length > 1) {
            t.push("".concat(n("plural"), "([")),
            t.indent(i());
            const s = e.cases.length;
            for (let r = 0; r < s && (Ni(t, e.cases[r]),
            r !== s - 1); r++)
                t.push(", ");
            t.deindent(i()),
            t.push("])")
        }
    }
    function Wb(t, e) {
        e.body ? Ni(t, e.body) : t.push("null")
    }
    function Ni(t, e) {
        const {helper: n} = t;
        switch (e.type) {
        case 0:
            Wb(t, e);
            break;
        case 1:
            qb(t, e);
            break;
        case 2:
            Kb(t, e);
            break;
        case 6:
            Hb(t, e);
            break;
        case 8:
            t.push(JSON.stringify(e.value), e);
            break;
        case 7:
            t.push(JSON.stringify(e.value), e);
            break;
        case 5:
            t.push("".concat(n("interpolate"), "(").concat(n("list"), "(").concat(e.index, "))"), e);
            break;
        case 4:
            t.push("".concat(n("interpolate"), "(").concat(n("named"), "(").concat(JSON.stringify(e.key), "))"), e);
            break;
        case 9:
            t.push(JSON.stringify(e.value), e);
            break;
        case 3:
            t.push(JSON.stringify(e.value), e);
            break;
        default:
            throw Ui(ee.UNHANDLED_CODEGEN_NODE_TYPE, null, {
                domain: zb,
                args: [e.type]
            })
        }
    }
    const Yb = (t, e={}) => {
        const n = nc(e.mode) ? e.mode : "normal"
          , i = nc(e.filename) ? e.filename : "message.intl";
        e.sourceMap;
        const s = e.breakLineCode != null ? e.breakLineCode : n === "arrow" ? ";" : "\n"
          , r = e.needIndent ? e.needIndent : n !== "arrow"
          , o = t.helpers || []
          , a = Vb(t, {
            filename: i,
            breakLineCode: s,
            needIndent: r
        });
        a.push(n === "normal" ? "function __msg__ (ctx) {" : "(ctx) => {"),
        a.indent(r),
        o.length > 0 && (a.push("const { ".concat(bd(o.map(u => "".concat(u, ": _").concat(u)), ", "), " } = ctx")),
        a.newline()),
        a.push("return "),
        Ni(a, t),
        a.deindent(r),
        a.push("}"),
        delete t.helpers;
        const {code: l, map: c} = a.context();
        return {
            ast: t,
            code: l,
            map: c ? c.toJSON() : void 0
        }
    }
    ;
    function Xb(t, e={}) {
        const n = gd({}, e)
          , i = !!n.jit
          , s = !!n.minify
          , r = n.optimize == null ? !0 : n.optimize
          , a = Fb(n).parse(t);
        return i ? (r && jb(a),
        s && gi(a),
        {
            ast: a,
            code: ""
        }) : (Ub(a, n),
        Yb(a, n))
    }
    /*!
  * core-base v9.14.2
  * (c) 2024 kazuya kawaguchi
  * Released under the MIT License.
  */
    function Jb() {
        typeof __INTLIFY_PROD_DEVTOOLS__ != "boolean" && (hn().__INTLIFY_PROD_DEVTOOLS__ = !1),
        typeof __INTLIFY_JIT_COMPILATION__ != "boolean" && (hn().__INTLIFY_JIT_COMPILATION__ = !1),
        typeof __INTLIFY_DROP_MESSAGE_COMPILER__ != "boolean" && (hn().__INTLIFY_DROP_MESSAGE_COMPILER__ = !1)
    }
    const Gn = [];
    Gn[0] = {
        w: [0],
        i: [3, 0],
        "[": [4],
        o: [7]
    };
    Gn[1] = {
        w: [1],
        ".": [2],
        "[": [4],
        o: [7]
    };
    Gn[2] = {
        w: [2],
        i: [3, 0],
        0: [3, 0]
    };
    Gn[3] = {
        i: [3, 0],
        0: [3, 0],
        w: [1, 1],
        ".": [2, 1],
        "[": [4, 1],
        o: [7, 1]
    };
    Gn[4] = {
        "'": [5, 0],
        '"': [6, 0],
        "[": [4, 2],
        "]": [1, 3],
        o: 8,
        l: [4, 0]
    };
    Gn[5] = {
        "'": [4, 0],
        o: 8,
        l: [5, 0]
    };
    Gn[6] = {
        '"': [4, 0],
        o: 8,
        l: [6, 0]
    };
    const Qb = /^\s?(?:true|false|-?[\d.]+|'[^']*'|"[^"]*")\s?$/;
    function Zb(t) {
        return Qb.test(t)
    }
    function ey(t) {
        const e = t.charCodeAt(0)
          , n = t.charCodeAt(t.length - 1);
        return e === n && (e === 34 || e === 39) ? t.slice(1, -1) : t
    }
    function ty(t) {
        if (t == null)
            return "o";
        switch (t.charCodeAt(0)) {
        case 91:
        case 93:
        case 46:
        case 34:
        case 39:
            return t;
        case 95:
        case 36:
        case 45:
            return "i";
        case 9:
        case 10:
        case 13:
        case 160:
        case 65279:
        case 8232:
        case 8233:
            return "w"
        }
        return "i"
    }
    function ny(t) {
        const e = t.trim();
        return t.charAt(0) === "0" && isNaN(parseInt(t)) ? !1 : Zb(e) ? ey(e) : "*" + e
    }
    function iy(t) {
        const e = [];
        let n = -1, i = 0, s = 0, r, o, a, l, c, u, f;
        const d = [];
        d[0] = () => {
            o === void 0 ? o = a : o += a
        }
        ,
        d[1] = () => {
            o !== void 0 && (e.push(o),
            o = void 0)
        }
        ,
        d[2] = () => {
            d[0](),
            s++
        }
        ,
        d[3] = () => {
            if (s > 0)
                s--,
                i = 4,
                d[0]();
            else {
                if (s = 0,
                o === void 0 || (o = ny(o),
                o === !1))
                    return !1;
                d[1]()
            }
        }
        ;
        function _() {
            const m = t[n + 1];
            if (i === 5 && m === "'" || i === 6 && m === '"')
                return n++,
                a = "\\" + m,
                d[0](),
                !0
        }
        for (; i !== null; )
            if (n++,
            r = t[n],
            !(r === "\\" && _())) {
                if (l = ty(r),
                f = Gn[i],
                c = f[l] || f.l || 8,
                c === 8 || (i = c[0],
                c[1] !== void 0 && (u = d[c[1]],
                u && (a = r,
                u() === !1))))
                    return;
                if (i === 7)
                    return e
            }
    }
    const oc = new Map;
    function sy(t, e) {
        return fe(t) ? t[e] : null
    }
    function ry(t, e) {
        if (!fe(t))
            return null;
        let n = oc.get(e);
        if (n || (n = iy(e),
        n && oc.set(e, n)),
        !n)
            return null;
        const i = n.length;
        let s = t
          , r = 0;
        for (; r < i; ) {
            const o = s[n[r]];
            if (o === void 0 || $e(s))
                return null;
            s = o,
            r++
        }
        return s
    }
    const oy = t => t
      , ay = t => ""
      , ly = "text"
      , cy = t => t.length === 0 ? "" : yb(t)
      , uy = bb;
    function ac(t, e) {
        return t = Math.abs(t),
        e === 2 ? t ? t > 1 ? 1 : 0 : 1 : t ? Math.min(t, 2) : 0
    }
    function fy(t) {
        const e = De(t.pluralIndex) ? t.pluralIndex : -1;
        return t.named && (De(t.named.count) || De(t.named.n)) ? De(t.named.count) ? t.named.count : De(t.named.n) ? t.named.n : e : e
    }
    function dy(t, e) {
        e.count || (e.count = t),
        e.n || (e.n = t)
    }
    function py(t={}) {
        const e = t.locale
          , n = fy(t)
          , i = fe(t.pluralRules) && H(e) && $e(t.pluralRules[e]) ? t.pluralRules[e] : ac
          , s = fe(t.pluralRules) && H(e) && $e(t.pluralRules[e]) ? ac : void 0
          , r = E => E[i(n, E.length, s)]
          , o = t.list || []
          , a = E => o[E]
          , l = t.named || ye();
        De(t.pluralIndex) && dy(n, l);
        const c = E => l[E];
        function u(E) {
            const S = $e(t.messages) ? t.messages(E) : fe(t.messages) ? t.messages[E] : !1;
            return S || (t.parent ? t.parent.message(E) : ay)
        }
        const f = E => t.modifiers ? t.modifiers[E] : oy
          , d = ne(t.processor) && $e(t.processor.normalize) ? t.processor.normalize : cy
          , _ = ne(t.processor) && $e(t.processor.interpolate) ? t.processor.interpolate : uy
          , m = ne(t.processor) && H(t.processor.type) ? t.processor.type : ly
          , b = {
            list: a,
            named: c,
            plural: r,
            linked: (E, ...S) => {
                const [v,y] = S;
                let A = "text"
                  , T = "";
                S.length === 1 ? fe(v) ? (T = v.modifier || T,
                A = v.type || A) : H(v) && (T = v || T) : S.length === 2 && (H(v) && (T = v || T),
                H(y) && (A = y || A));
                const O = u(E)(b)
                  , C = A === "vnode" && Te(O) && T ? O[0] : O;
                return T ? f(T)(C, A) : C
            }
            ,
            message: u,
            type: m,
            interpolate: _,
            normalize: d,
            values: Ye(ye(), o, l)
        };
        return b
    }
    let ks = null;
    function _y(t) {
        ks = t
    }
    function hy(t, e, n) {
        ks && ks.emit("i18n:init", {
            timestamp: Date.now(),
            i18n: t,
            version: e,
            meta: n
        })
    }
    const my = gy("function:translate");
    function gy(t) {
        return e => ks && ks.emit(t, e)
    }
    const by = Ma.__EXTEND_POINT__
      , Wn = xr(by)
      , yy = {
        FALLBACK_TO_TRANSLATE: Wn(),
        CANNOT_FORMAT_NUMBER: Wn(),
        FALLBACK_TO_NUMBER_FORMAT: Wn(),
        CANNOT_FORMAT_DATE: Wn(),
        FALLBACK_TO_DATE_FORMAT: Wn(),
        EXPERIMENTAL_CUSTOM_MESSAGE_COMPILER: Wn(),
        __EXTEND_POINT__: Wn()
    }
      , yd = ee.__EXTEND_POINT__
      , Yn = xr(yd)
      , jt = {
        INVALID_ARGUMENT: yd,
        INVALID_DATE_ARGUMENT: Yn(),
        INVALID_ISO_DATE_ARGUMENT: Yn(),
        NOT_SUPPORT_NON_STRING_MESSAGE: Yn(),
        NOT_SUPPORT_LOCALE_PROMISE_VALUE: Yn(),
        NOT_SUPPORT_LOCALE_ASYNC_FUNCTION: Yn(),
        NOT_SUPPORT_LOCALE_TYPE: Yn(),
        __EXTEND_POINT__: Yn()
    };
    function Zt(t) {
        return Ui(t, null, void 0)
    }
    function Ga(t, e) {
        return e.locale != null ? lc(e.locale) : lc(t.locale)
    }
    let ro;
    function lc(t) {
        if (H(t))
            return t;
        if ($e(t)) {
            if (t.resolvedOnce && ro != null)
                return ro;
            if (t.constructor.name === "Function") {
                const e = t();
                if (gb(e))
                    throw Zt(jt.NOT_SUPPORT_LOCALE_PROMISE_VALUE);
                return ro = e
            } else
                throw Zt(jt.NOT_SUPPORT_LOCALE_ASYNC_FUNCTION)
        } else
            throw Zt(jt.NOT_SUPPORT_LOCALE_TYPE)
    }
    function vy(t, e, n) {
        return [...new Set([n, ...Te(e) ? e : fe(e) ? Object.keys(e) : H(e) ? [e] : [n]])]
    }
    function vd(t, e, n) {
        const i = H(n) ? n : Di
          , s = t;
        s.__localeChainCache || (s.__localeChainCache = new Map);
        let r = s.__localeChainCache.get(i);
        if (!r) {
            r = [];
            let o = [n];
            for (; Te(o); )
                o = cc(r, o, e);
            const a = Te(e) || !ne(e) ? e : e.default ? e.default : null;
            o = H(a) ? [a] : a,
            Te(o) && cc(r, o, !1),
            s.__localeChainCache.set(i, r)
        }
        return r
    }
    function cc(t, e, n) {
        let i = !0;
        for (let s = 0; s < e.length && oe(i); s++) {
            const r = e[s];
            H(r) && (i = ky(t, e[s], n))
        }
        return i
    }
    function ky(t, e, n) {
        let i;
        const s = e.split("-");
        do {
            const r = s.join("-");
            i = Ay(t, r, n),
            s.splice(-1, 1)
        } while (s.length && i === !0);
        return i
    }
    function Ay(t, e, n) {
        let i = !1;
        if (!t.includes(e) && (i = !0,
        e)) {
            i = e[e.length - 1] !== "!";
            const s = e.replace(/!/g, "");
            t.push(s),
            (Te(n) || ne(n)) && n[s] && (i = n[s])
        }
        return i
    }
    const $y = "9.14.2"
      , Ir = -1
      , Di = "en-US"
      , uc = ""
      , fc = t => "".concat(t.charAt(0).toLocaleUpperCase()).concat(t.substr(1));
    function Ty() {
        return {
            upper: (t, e) => e === "text" && H(t) ? t.toUpperCase() : e === "vnode" && fe(t) && "__v_isVNode"in t ? t.children.toUpperCase() : t,
            lower: (t, e) => e === "text" && H(t) ? t.toLowerCase() : e === "vnode" && fe(t) && "__v_isVNode"in t ? t.children.toLowerCase() : t,
            capitalize: (t, e) => e === "text" && H(t) ? fc(t) : e === "vnode" && fe(t) && "__v_isVNode"in t ? fc(t.children) : t
        }
    }
    let kd;
    function dc(t) {
        kd = t
    }
    let Ad;
    function Ey(t) {
        Ad = t
    }
    let $d;
    function wy(t) {
        $d = t
    }
    let Td = null;
    const Sy = t => {
        Td = t
    }
      , Oy = () => Td;
    let Ed = null;
    const pc = t => {
        Ed = t
    }
      , Py = () => Ed;
    let _c = 0;
    function xy(t={}) {
        const e = $e(t.onWarn) ? t.onWarn : vb
          , n = H(t.version) ? t.version : $y
          , i = H(t.locale) || $e(t.locale) ? t.locale : Di
          , s = $e(i) ? Di : i
          , r = Te(t.fallbackLocale) || ne(t.fallbackLocale) || H(t.fallbackLocale) || t.fallbackLocale === !1 ? t.fallbackLocale : s
          , o = ne(t.messages) ? t.messages : oo(s)
          , a = ne(t.datetimeFormats) ? t.datetimeFormats : oo(s)
          , l = ne(t.numberFormats) ? t.numberFormats : oo(s)
          , c = Ye(ye(), t.modifiers, Ty())
          , u = t.pluralRules || ye()
          , f = $e(t.missing) ? t.missing : null
          , d = oe(t.missingWarn) || Rn(t.missingWarn) ? t.missingWarn : !0
          , _ = oe(t.fallbackWarn) || Rn(t.fallbackWarn) ? t.fallbackWarn : !0
          , m = !!t.fallbackFormat
          , p = !!t.unresolving
          , b = $e(t.postTranslation) ? t.postTranslation : null
          , E = ne(t.processor) ? t.processor : null
          , S = oe(t.warnHtmlMessage) ? t.warnHtmlMessage : !0
          , v = !!t.escapeParameter
          , y = $e(t.messageCompiler) ? t.messageCompiler : kd
          , A = $e(t.messageResolver) ? t.messageResolver : Ad || sy
          , T = $e(t.localeFallbacker) ? t.localeFallbacker : $d || vy
          , O = fe(t.fallbackContext) ? t.fallbackContext : void 0
          , C = t
          , P = fe(C.__datetimeFormatters) ? C.__datetimeFormatters : new Map
          , W = fe(C.__numberFormatters) ? C.__numberFormatters : new Map
          , X = fe(C.__meta) ? C.__meta : {};
        _c++;
        const q = {
            version: n,
            cid: _c,
            locale: i,
            fallbackLocale: r,
            messages: o,
            modifiers: c,
            pluralRules: u,
            missing: f,
            missingWarn: d,
            fallbackWarn: _,
            fallbackFormat: m,
            unresolving: p,
            postTranslation: b,
            processor: E,
            warnHtmlMessage: S,
            escapeParameter: v,
            messageCompiler: y,
            messageResolver: A,
            localeFallbacker: T,
            fallbackContext: O,
            onWarn: e,
            __meta: X
        };
        return q.datetimeFormats = a,
        q.numberFormats = l,
        q.__datetimeFormatters = P,
        q.__numberFormatters = W,
        __INTLIFY_PROD_DEVTOOLS__ && hy(q, n, X),
        q
    }
    const oo = t => ({
        [t]: ye()
    });
    function Ua(t, e, n, i, s) {
        const {missing: r, onWarn: o} = t;
        if (r !== null) {
            const a = r(t, n, e, s);
            return H(a) ? a : e
        } else
            return e
    }
    function Hi(t, e, n) {
        const i = t;
        i.__localeChainCache = new Map,
        t.localeFallbacker(t, n, e)
    }
    function Iy(t, e) {
        return t === e ? !1 : t.split("-")[0] === e.split("-")[0]
    }
    function Cy(t, e) {
        const n = e.indexOf(t);
        if (n === -1)
            return !1;
        for (let i = n + 1; i < e.length; i++)
            if (Iy(t, e[i]))
                return !0;
        return !1
    }
    function ao(t) {
        return n => Ry(n, t)
    }
    function Ry(t, e) {
        const n = Ny(e);
        if (n == null)
            throw As(0);
        if (ja(n) === 1) {
            const r = My(n);
            return t.plural(r.reduce( (o, a) => [...o, hc(t, a)], []))
        } else
            return hc(t, n)
    }
    const Ly = ["b", "body"];
    function Ny(t) {
        return Un(t, Ly)
    }
    const Dy = ["c", "cases"];
    function My(t) {
        return Un(t, Dy, [])
    }
    function hc(t, e) {
        const n = Gy(e);
        if (n != null)
            return t.type === "text" ? n : t.normalize([n]);
        {
            const i = jy(e).reduce( (s, r) => [...s, jo(t, r)], []);
            return t.normalize(i)
        }
    }
    const Fy = ["s", "static"];
    function Gy(t) {
        return Un(t, Fy)
    }
    const Uy = ["i", "items"];
    function jy(t) {
        return Un(t, Uy, [])
    }
    function jo(t, e) {
        const n = ja(e);
        switch (n) {
        case 3:
            return Ms(e, n);
        case 9:
            return Ms(e, n);
        case 4:
            {
                const i = e;
                if (Ut(i, "k") && i.k)
                    return t.interpolate(t.named(i.k));
                if (Ut(i, "key") && i.key)
                    return t.interpolate(t.named(i.key));
                throw As(n)
            }
        case 5:
            {
                const i = e;
                if (Ut(i, "i") && De(i.i))
                    return t.interpolate(t.list(i.i));
                if (Ut(i, "index") && De(i.index))
                    return t.interpolate(t.list(i.index));
                throw As(n)
            }
        case 6:
            {
                const i = e
                  , s = Hy(i)
                  , r = qy(i);
                return t.linked(jo(t, r), s ? jo(t, s) : void 0, t.type)
            }
        case 7:
            return Ms(e, n);
        case 8:
            return Ms(e, n);
        default:
            throw new Error("unhandled node on format message part: ".concat(n))
        }
    }
    const By = ["t", "type"];
    function ja(t) {
        return Un(t, By)
    }
    const zy = ["v", "value"];
    function Ms(t, e) {
        const n = Un(t, zy);
        if (n)
            return n;
        throw As(e)
    }
    const Vy = ["m", "modifier"];
    function Hy(t) {
        return Un(t, Vy)
    }
    const Ky = ["k", "key"];
    function qy(t) {
        const e = Un(t, Ky);
        if (e)
            return e;
        throw As(6)
    }
    function Un(t, e, n) {
        for (let i = 0; i < e.length; i++) {
            const s = e[i];
            if (Ut(t, s) && t[s] != null)
                return t[s]
        }
        return n
    }
    function As(t) {
        return new Error("unhandled node type: ".concat(t))
    }
    const wd = t => t;
    let vi = ye();
    function Mi(t) {
        return fe(t) && ja(t) === 0 && (Ut(t, "b") || Ut(t, "body"))
    }
    function Sd(t, e={}) {
        let n = !1;
        const i = e.onError || Sb;
        return e.onError = s => {
            n = !0,
            i(s)
        }
        ,
        Is(Bn({}, Xb(t, e)), {
            detectError: n
        })
    }
    const Wy = (t, e) => {
        if (!H(t))
            throw Zt(jt.NOT_SUPPORT_NON_STRING_MESSAGE);
        {
            oe(e.warnHtmlMessage) && e.warnHtmlMessage;
            const i = (e.onCacheKey || wd)(t)
              , s = vi[i];
            if (s)
                return s;
            const {code: r, detectError: o} = Sd(t, e)
              , a = new Function("return ".concat(r))();
            return o ? a : vi[i] = a
        }
    }
    ;
    function Yy(t, e) {
        if (__INTLIFY_JIT_COMPILATION__ && !__INTLIFY_DROP_MESSAGE_COMPILER__ && H(t)) {
            oe(e.warnHtmlMessage) && e.warnHtmlMessage;
            const i = (e.onCacheKey || wd)(t)
              , s = vi[i];
            if (s)
                return s;
            const {ast: r, detectError: o} = Sd(t, Is(Bn({}, e), {
                location: !1,
                jit: !0
            }))
              , a = ao(r);
            return o ? a : vi[i] = a
        } else {
            const n = t.cacheKey;
            if (n) {
                const i = vi[n];
                return i || (vi[n] = ao(t))
            } else
                return ao(t)
        }
    }
    const mc = () => ""
      , Lt = t => $e(t);
    function gc(t, ...e) {
        const {fallbackFormat: n, postTranslation: i, unresolving: s, messageCompiler: r, fallbackLocale: o, messages: a} = t
          , [l,c] = Bo(...e)
          , u = oe(c.missingWarn) ? c.missingWarn : t.missingWarn
          , f = oe(c.fallbackWarn) ? c.fallbackWarn : t.fallbackWarn
          , d = oe(c.escapeParameter) ? c.escapeParameter : t.escapeParameter
          , _ = !!c.resolvedMessage
          , m = H(c.default) || oe(c.default) ? oe(c.default) ? r ? l : () => l : c.default : n ? r ? l : () => l : ""
          , p = n || m !== ""
          , b = Ga(t, c);
        d && Xy(c);
        let[E,S,v] = _ ? [l, b, a[b] || ye()] : Od(t, l, b, o, f, u)
          , y = E
          , A = l;
        if (!_ && !(H(y) || Mi(y) || Lt(y)) && p && (y = m,
        A = y),
        !_ && (!(H(y) || Mi(y) || Lt(y)) || !H(S)))
            return s ? Ir : l;
        let T = !1;
        const O = () => {
            T = !0
        }
          , C = Lt(y) ? y : Pd(t, l, S, y, A, O);
        if (T)
            return y;
        const P = Zy(t, S, v, c)
          , W = py(P)
          , X = Jy(t, C, W)
          , q = i ? i(X, l) : X;
        if (__INTLIFY_PROD_DEVTOOLS__) {
            const Q = {
                timestamp: Date.now(),
                key: H(l) ? l : Lt(y) ? y.key : "",
                locale: S || (Lt(y) ? y.locale : ""),
                format: H(y) ? y : Lt(y) ? y.source : "",
                message: q
            };
            Q.meta = Ye({}, t.__meta, Oy() || {}),
            my(Q)
        }
        return q
    }
    function Xy(t) {
        Te(t.list) ? t.list = t.list.map(e => H(e) ? tc(e) : e) : fe(t.named) && Object.keys(t.named).forEach(e => {
            H(t.named[e]) && (t.named[e] = tc(t.named[e]))
        }
        )
    }
    function Od(t, e, n, i, s, r) {
        const {messages: o, onWarn: a, messageResolver: l, localeFallbacker: c} = t
          , u = c(t, i, n);
        let f = ye(), d, _ = null;
        const m = "translate";
        for (let p = 0; p < u.length && (d = u[p],
        f = o[d] || ye(),
        (_ = l(f, e)) === null && (_ = f[e]),
        !(H(_) || Mi(_) || Lt(_))); p++)
            if (!Cy(d, u)) {
                const b = Ua(t, e, d, r, m);
                b !== e && (_ = b)
            }
        return [_, d, f]
    }
    function Pd(t, e, n, i, s, r) {
        const {messageCompiler: o, warnHtmlMessage: a} = t;
        if (Lt(i)) {
            const c = i;
            return c.locale = c.locale || n,
            c.key = c.key || e,
            c
        }
        if (o == null) {
            const c = () => i;
            return c.locale = n,
            c.key = e,
            c
        }
        const l = o(i, Qy(t, n, s, i, a, r));
        return l.locale = n,
        l.key = e,
        l.source = i,
        l
    }
    function Jy(t, e, n) {
        return e(n)
    }
    function Bo(...t) {
        const [e,n,i] = t
          , s = ye();
        if (!H(e) && !De(e) && !Lt(e) && !Mi(e))
            throw Zt(jt.INVALID_ARGUMENT);
        const r = De(e) ? String(e) : (Lt(e),
        e);
        return De(n) ? s.plural = n : H(n) ? s.default = n : ne(n) && !Pr(n) ? s.named = n : Te(n) && (s.list = n),
        De(i) ? s.plural = i : H(i) ? s.default = i : ne(i) && Ye(s, i),
        [r, s]
    }
    function Qy(t, e, n, i, s, r) {
        return {
            locale: e,
            key: n,
            warnHtmlMessage: s,
            onError: o => {
                throw r && r(o),
                o
            }
            ,
            onCacheKey: o => db(e, n, o)
        }
    }
    function Zy(t, e, n, i) {
        const {modifiers: s, pluralRules: r, messageResolver: o, fallbackLocale: a, fallbackWarn: l, missingWarn: c, fallbackContext: u} = t
          , d = {
            locale: e,
            modifiers: s,
            pluralRules: r,
            messages: _ => {
                let m = o(n, _);
                if (m == null && u) {
                    const [,,p] = Od(u, _, e, a, l, c);
                    m = o(p, _)
                }
                if (H(m) || Mi(m)) {
                    let p = !1;
                    const E = Pd(t, _, e, m, _, () => {
                        p = !0
                    }
                    );
                    return p ? mc : E
                } else
                    return Lt(m) ? m : mc
            }
        };
        return t.processor && (d.processor = t.processor),
        i.list && (d.list = i.list),
        i.named && (d.named = i.named),
        De(i.plural) && (d.pluralIndex = i.plural),
        d
    }
    function bc(t, ...e) {
        const {datetimeFormats: n, unresolving: i, fallbackLocale: s, onWarn: r, localeFallbacker: o} = t
          , {__datetimeFormatters: a} = t
          , [l,c,u,f] = zo(...e)
          , d = oe(u.missingWarn) ? u.missingWarn : t.missingWarn;
        oe(u.fallbackWarn) ? u.fallbackWarn : t.fallbackWarn;
        const _ = !!u.part
          , m = Ga(t, u)
          , p = o(t, s, m);
        if (!H(l) || l === "")
            return new Intl.DateTimeFormat(m,f).format(c);
        let b = {}, E, S = null;
        const v = "datetime format";
        for (let T = 0; T < p.length && (E = p[T],
        b = n[E] || {},
        S = b[l],
        !ne(S)); T++)
            Ua(t, l, E, d, v);
        if (!ne(S) || !H(E))
            return i ? Ir : l;
        let y = "".concat(E, "__").concat(l);
        Pr(f) || (y = "".concat(y, "__").concat(JSON.stringify(f)));
        let A = a.get(y);
        return A || (A = new Intl.DateTimeFormat(E,Ye({}, S, f)),
        a.set(y, A)),
        _ ? A.formatToParts(c) : A.format(c)
    }
    const xd = ["localeMatcher", "weekday", "era", "year", "month", "day", "hour", "minute", "second", "timeZoneName", "formatMatcher", "hour12", "timeZone", "dateStyle", "timeStyle", "calendar", "dayPeriod", "numberingSystem", "hourCycle", "fractionalSecondDigits"];
    function zo(...t) {
        const [e,n,i,s] = t
          , r = ye();
        let o = ye(), a;
        if (H(e)) {
            const l = e.match(/(\d{4}-\d{2}-\d{2})(T|\s)?(.*)/);
            if (!l)
                throw Zt(jt.INVALID_ISO_DATE_ARGUMENT);
            const c = l[3] ? l[3].trim().startsWith("T") ? "".concat(l[1].trim()).concat(l[3].trim()) : "".concat(l[1].trim(), "T").concat(l[3].trim()) : l[1].trim();
            a = new Date(c);
            try {
                a.toISOString()
            } catch (u) {
                throw Zt(jt.INVALID_ISO_DATE_ARGUMENT)
            }
        } else if (_b(e)) {
            if (isNaN(e.getTime()))
                throw Zt(jt.INVALID_DATE_ARGUMENT);
            a = e
        } else if (De(e))
            a = e;
        else
            throw Zt(jt.INVALID_ARGUMENT);
        return H(n) ? r.key = n : ne(n) && Object.keys(n).forEach(l => {
            xd.includes(l) ? o[l] = n[l] : r[l] = n[l]
        }
        ),
        H(i) ? r.locale = i : ne(i) && (o = i),
        ne(s) && (o = s),
        [r.key || "", a, r, o]
    }
    function yc(t, e, n) {
        const i = t;
        for (const s in n) {
            const r = "".concat(e, "__").concat(s);
            i.__datetimeFormatters.has(r) && i.__datetimeFormatters.delete(r)
        }
    }
    function vc(t, ...e) {
        const {numberFormats: n, unresolving: i, fallbackLocale: s, onWarn: r, localeFallbacker: o} = t
          , {__numberFormatters: a} = t
          , [l,c,u,f] = Vo(...e)
          , d = oe(u.missingWarn) ? u.missingWarn : t.missingWarn;
        oe(u.fallbackWarn) ? u.fallbackWarn : t.fallbackWarn;
        const _ = !!u.part
          , m = Ga(t, u)
          , p = o(t, s, m);
        if (!H(l) || l === "")
            return new Intl.NumberFormat(m,f).format(c);
        let b = {}, E, S = null;
        const v = "number format";
        for (let T = 0; T < p.length && (E = p[T],
        b = n[E] || {},
        S = b[l],
        !ne(S)); T++)
            Ua(t, l, E, d, v);
        if (!ne(S) || !H(E))
            return i ? Ir : l;
        let y = "".concat(E, "__").concat(l);
        Pr(f) || (y = "".concat(y, "__").concat(JSON.stringify(f)));
        let A = a.get(y);
        return A || (A = new Intl.NumberFormat(E,Ye({}, S, f)),
        a.set(y, A)),
        _ ? A.formatToParts(c) : A.format(c)
    }
    const Id = ["localeMatcher", "style", "currency", "currencyDisplay", "currencySign", "useGrouping", "minimumIntegerDigits", "minimumFractionDigits", "maximumFractionDigits", "minimumSignificantDigits", "maximumSignificantDigits", "compactDisplay", "notation", "signDisplay", "unit", "unitDisplay", "roundingMode", "roundingPriority", "roundingIncrement", "trailingZeroDisplay"];
    function Vo(...t) {
        const [e,n,i,s] = t
          , r = ye();
        let o = ye();
        if (!De(e))
            throw Zt(jt.INVALID_ARGUMENT);
        const a = e;
        return H(n) ? r.key = n : ne(n) && Object.keys(n).forEach(l => {
            Id.includes(l) ? o[l] = n[l] : r[l] = n[l]
        }
        ),
        H(i) ? r.locale = i : ne(i) && (o = i),
        ne(s) && (o = s),
        [r.key || "", a, r, o]
    }
    function kc(t, e, n) {
        const i = t;
        for (const s in n) {
            const r = "".concat(e, "__").concat(s);
            i.__numberFormatters.has(r) && i.__numberFormatters.delete(r)
        }
    }
    Jb();
    /*!
  * vue-i18n v9.14.2
  * (c) 2024 kazuya kawaguchi
  * Released under the MIT License.
  */
    const ev = "9.14.2";
    function tv() {
        typeof __VUE_I18N_FULL_INSTALL__ != "boolean" && (hn().__VUE_I18N_FULL_INSTALL__ = !0),
        typeof __VUE_I18N_LEGACY_API__ != "boolean" && (hn().__VUE_I18N_LEGACY_API__ = !0),
        typeof __INTLIFY_JIT_COMPILATION__ != "boolean" && (hn().__INTLIFY_JIT_COMPILATION__ = !1),
        typeof __INTLIFY_DROP_MESSAGE_COMPILER__ != "boolean" && (hn().__INTLIFY_DROP_MESSAGE_COMPILER__ = !1),
        typeof __INTLIFY_PROD_DEVTOOLS__ != "boolean" && (hn().__INTLIFY_PROD_DEVTOOLS__ = !1)
    }
    const nv = yy.__EXTEND_POINT__
      , un = xr(nv);
    un(),
    un(),
    un(),
    un(),
    un(),
    un(),
    un(),
    un(),
    un();
    const Cd = jt.__EXTEND_POINT__
      , pt = xr(Cd)
      , je = {
        UNEXPECTED_RETURN_TYPE: Cd,
        INVALID_ARGUMENT: pt(),
        MUST_BE_CALL_SETUP_TOP: pt(),
        NOT_INSTALLED: pt(),
        NOT_AVAILABLE_IN_LEGACY_MODE: pt(),
        REQUIRED_VALUE: pt(),
        INVALID_VALUE: pt(),
        CANNOT_SETUP_VUE_DEVTOOLS_PLUGIN: pt(),
        NOT_INSTALLED_WITH_PROVIDE: pt(),
        UNEXPECTED_ERROR: pt(),
        NOT_COMPATIBLE_LEGACY_VUE_I18N: pt(),
        BRIDGE_SUPPORT_VUE_2_ONLY: pt(),
        MUST_DEFINE_I18N_OPTION_IN_ALLOW_COMPOSITION: pt(),
        NOT_AVAILABLE_COMPOSITION_IN_LEGACY: pt(),
        __EXTEND_POINT__: pt()
    };
    function Ve(t, ...e) {
        return Ui(t, null, void 0)
    }
    const Ho = Fn("__translateVNode")
      , Ko = Fn("__datetimeParts")
      , qo = Fn("__numberParts")
      , Rd = Fn("__setPluralRules")
      , Ld = Fn("__injectWithOption")
      , Wo = Fn("__dispose");
    function $s(t) {
        if (!fe(t))
            return t;
        for (const e in t)
            if (Ut(t, e))
                if (!e.includes("."))
                    fe(t[e]) && $s(t[e]);
                else {
                    const n = e.split(".")
                      , i = n.length - 1;
                    let s = t
                      , r = !1;
                    for (let o = 0; o < i; o++) {
                        if (n[o]in s || (s[n[o]] = ye()),
                        !fe(s[n[o]])) {
                            r = !0;
                            break
                        }
                        s = s[n[o]]
                    }
                    r || (s[n[i]] = t[e],
                    delete t[e]),
                    fe(s[n[i]]) && $s(s[n[i]])
                }
        return t
    }
    function Cr(t, e) {
        const {messages: n, __i18n: i, messageResolver: s, flatJson: r} = e
          , o = ne(n) ? n : Te(i) ? ye() : {
            [t]: ye()
        };
        if (Te(i) && i.forEach(a => {
            if ("locale"in a && "resource"in a) {
                const {locale: l, resource: c} = a;
                l ? (o[l] = o[l] || ye(),
                Ws(c, o[l])) : Ws(c, o)
            } else
                H(a) && Ws(JSON.parse(a), o)
        }
        ),
        s == null && r)
            for (const a in o)
                Ut(o, a) && $s(o[a]);
        return o
    }
    function Nd(t) {
        return t.type
    }
    function Dd(t, e, n) {
        let i = fe(e.messages) ? e.messages : ye();
        "__i18nGlobal"in n && (i = Cr(t.locale.value, {
            messages: i,
            __i18n: n.__i18nGlobal
        }));
        const s = Object.keys(i);
        s.length && s.forEach(r => {
            t.mergeLocaleMessage(r, i[r])
        }
        );
        {
            if (fe(e.datetimeFormats)) {
                const r = Object.keys(e.datetimeFormats);
                r.length && r.forEach(o => {
                    t.mergeDateTimeFormat(o, e.datetimeFormats[o])
                }
                )
            }
            if (fe(e.numberFormats)) {
                const r = Object.keys(e.numberFormats);
                r.length && r.forEach(o => {
                    t.mergeNumberFormat(o, e.numberFormats[o])
                }
                )
            }
        }
    }
    function Ac(t) {
        return ft(Es, null, t, 0)
    }
    const $c = "__INTLIFY_META__"
      , Tc = () => []
      , iv = () => !1;
    let Ec = 0;
    function wc(t) {
        return (e, n, i, s) => t(n, i, fs() || void 0, s)
    }
    const sv = () => {
        const t = fs();
        let e = null;
        return t && (e = Nd(t)[$c]) ? {
            [$c]: e
        } : null
    }
    ;
    function Ba(t={}, e) {
        const {__root: n, __injectWithOption: i} = t
          , s = n === void 0
          , r = t.flatJson
          , o = fr ? Me : ru
          , a = !!t.translateExistCompatible;
        let l = oe(t.inheritLocale) ? t.inheritLocale : !0;
        const c = o(n && l ? n.locale.value : H(t.locale) ? t.locale : Di)
          , u = o(n && l ? n.fallbackLocale.value : H(t.fallbackLocale) || Te(t.fallbackLocale) || ne(t.fallbackLocale) || t.fallbackLocale === !1 ? t.fallbackLocale : c.value)
          , f = o(Cr(c.value, t))
          , d = o(ne(t.datetimeFormats) ? t.datetimeFormats : {
            [c.value]: {}
        })
          , _ = o(ne(t.numberFormats) ? t.numberFormats : {
            [c.value]: {}
        });
        let m = n ? n.missingWarn : oe(t.missingWarn) || Rn(t.missingWarn) ? t.missingWarn : !0
          , p = n ? n.fallbackWarn : oe(t.fallbackWarn) || Rn(t.fallbackWarn) ? t.fallbackWarn : !0
          , b = n ? n.fallbackRoot : oe(t.fallbackRoot) ? t.fallbackRoot : !0
          , E = !!t.fallbackFormat
          , S = $e(t.missing) ? t.missing : null
          , v = $e(t.missing) ? wc(t.missing) : null
          , y = $e(t.postTranslation) ? t.postTranslation : null
          , A = n ? n.warnHtmlMessage : oe(t.warnHtmlMessage) ? t.warnHtmlMessage : !0
          , T = !!t.escapeParameter;
        const O = n ? n.modifiers : ne(t.modifiers) ? t.modifiers : {};
        let C = t.pluralRules || n && n.pluralRules, P;
        P = ( () => {
            s && pc(null);
            const x = {
                version: ev,
                locale: c.value,
                fallbackLocale: u.value,
                messages: f.value,
                modifiers: O,
                pluralRules: C,
                missing: v === null ? void 0 : v,
                missingWarn: m,
                fallbackWarn: p,
                fallbackFormat: E,
                unresolving: !0,
                postTranslation: y === null ? void 0 : y,
                warnHtmlMessage: A,
                escapeParameter: T,
                messageResolver: t.messageResolver,
                messageCompiler: t.messageCompiler,
                __meta: {
                    framework: "vue"
                }
            };
            x.datetimeFormats = d.value,
            x.numberFormats = _.value,
            x.__datetimeFormatters = ne(P) ? P.__datetimeFormatters : void 0,
            x.__numberFormatters = ne(P) ? P.__numberFormatters : void 0;
            const D = xy(x);
            return s && pc(D),
            D
        }
        )(),
        Hi(P, c.value, u.value);
        function X() {
            return [c.value, u.value, f.value, d.value, _.value]
        }
        const q = Ft({
            get: () => c.value,
            set: x => {
                c.value = x,
                P.locale = c.value
            }
        })
          , Q = Ft({
            get: () => u.value,
            set: x => {
                u.value = x,
                P.fallbackLocale = u.value,
                Hi(P, c.value, x)
            }
        })
          , _e = Ft( () => f.value)
          , ue = Ft( () => d.value)
          , Z = Ft( () => _.value);
        function J() {
            return $e(y) ? y : null
        }
        function te(x) {
            y = x,
            P.postTranslation = x
        }
        function Ie() {
            return S
        }
        function ze(x) {
            x !== null && (v = wc(x)),
            S = x,
            P.missing = v
        }
        const he = (x, D, Y, ae, we, st) => {
            X();
            let Ke;
            try {
                __INTLIFY_PROD_DEVTOOLS__,
                s || (P.fallbackContext = n ? Py() : void 0),
                Ke = x(P)
            } finally {
                __INTLIFY_PROD_DEVTOOLS__,
                s || (P.fallbackContext = void 0)
            }
            if (Y !== "translate exists" && De(Ke) && Ke === Ir || Y === "translate exists" && !Ke) {
                const [jn,Lr] = D();
                return n && b ? ae(n) : we(jn)
            } else {
                if (st(Ke))
                    return Ke;
                throw Ve(je.UNEXPECTED_RETURN_TYPE)
            }
        }
        ;
        function me(...x) {
            return he(D => Reflect.apply(gc, null, [D, ...x]), () => Bo(...x), "translate", D => Reflect.apply(D.t, D, [...x]), D => D, D => H(D))
        }
        function vt(...x) {
            const [D,Y,ae] = x;
            if (ae && !fe(ae))
                throw Ve(je.INVALID_ARGUMENT);
            return me(D, Y, Ye({
                resolvedMessage: !0
            }, ae || {}))
        }
        function G(...x) {
            return he(D => Reflect.apply(bc, null, [D, ...x]), () => zo(...x), "datetime format", D => Reflect.apply(D.d, D, [...x]), () => uc, D => H(D))
        }
        function K(...x) {
            return he(D => Reflect.apply(vc, null, [D, ...x]), () => Vo(...x), "number format", D => Reflect.apply(D.n, D, [...x]), () => uc, D => H(D))
        }
        function ge(x) {
            return x.map(D => H(D) || De(D) || oe(D) ? Ac(String(D)) : D)
        }
        const Ue = {
            normalize: ge,
            interpolate: x => x,
            type: "vnode"
        };
        function Ce(...x) {
            return he(D => {
                let Y;
                const ae = D;
                try {
                    ae.processor = Ue,
                    Y = Reflect.apply(gc, null, [ae, ...x])
                } finally {
                    ae.processor = null
                }
                return Y
            }
            , () => Bo(...x), "translate", D => D[Ho](...x), D => [Ac(D)], D => Te(D))
        }
        function Re(...x) {
            return he(D => Reflect.apply(vc, null, [D, ...x]), () => Vo(...x), "number format", D => D[qo](...x), Tc, D => H(D) || Te(D))
        }
        function on(...x) {
            return he(D => Reflect.apply(bc, null, [D, ...x]), () => zo(...x), "datetime format", D => D[Ko](...x), Tc, D => H(D) || Te(D))
        }
        function k(x) {
            C = x,
            P.pluralRules = C
        }
        function w(x, D) {
            return he( () => {
                if (!x)
                    return !1;
                const Y = H(D) ? D : c.value
                  , ae = M(Y)
                  , we = P.messageResolver(ae, x);
                return a ? we != null : Mi(we) || Lt(we) || H(we)
            }
            , () => [x], "translate exists", Y => Reflect.apply(Y.te, Y, [x, D]), iv, Y => oe(Y))
        }
        function $(x) {
            let D = null;
            const Y = vd(P, u.value, c.value);
            for (let ae = 0; ae < Y.length; ae++) {
                const we = f.value[Y[ae]] || {}
                  , st = P.messageResolver(we, x);
                if (st != null) {
                    D = st;
                    break
                }
            }
            return D
        }
        function N(x) {
            const D = $(x);
            return D != null ? D : n ? n.tm(x) || {} : {}
        }
        function M(x) {
            return f.value[x] || {}
        }
        function F(x, D) {
            if (r) {
                const Y = {
                    [x]: D
                };
                for (const ae in Y)
                    Ut(Y, ae) && $s(Y[ae]);
                D = Y[x]
            }
            f.value[x] = D,
            P.messages = f.value
        }
        function z(x, D) {
            f.value[x] = f.value[x] || {};
            const Y = {
                [x]: D
            };
            if (r)
                for (const ae in Y)
                    Ut(Y, ae) && $s(Y[ae]);
            D = Y[x],
            Ws(D, f.value[x]),
            P.messages = f.value
        }
        function B(x) {
            return d.value[x] || {}
        }
        function h(x, D) {
            d.value[x] = D,
            P.datetimeFormats = d.value,
            yc(P, x, D)
        }
        function g(x, D) {
            d.value[x] = Ye(d.value[x] || {}, D),
            P.datetimeFormats = d.value,
            yc(P, x, D)
        }
        function I(x) {
            return _.value[x] || {}
        }
        function L(x, D) {
            _.value[x] = D,
            P.numberFormats = _.value,
            kc(P, x, D)
        }
        function V(x, D) {
            _.value[x] = Ye(_.value[x] || {}, D),
            P.numberFormats = _.value,
            kc(P, x, D)
        }
        Ec++,
        n && fr && (Sn(n.locale, x => {
            l && (c.value = x,
            P.locale = x,
            Hi(P, c.value, u.value))
        }
        ),
        Sn(n.fallbackLocale, x => {
            l && (u.value = x,
            P.fallbackLocale = x,
            Hi(P, c.value, u.value))
        }
        ));
        const U = {
            id: Ec,
            locale: q,
            fallbackLocale: Q,
            get inheritLocale() {
                return l
            },
            set inheritLocale(x) {
                l = x,
                x && n && (c.value = n.locale.value,
                u.value = n.fallbackLocale.value,
                Hi(P, c.value, u.value))
            },
            get availableLocales() {
                return Object.keys(f.value).sort()
            },
            messages: _e,
            get modifiers() {
                return O
            },
            get pluralRules() {
                return C || {}
            },
            get isGlobal() {
                return s
            },
            get missingWarn() {
                return m
            },
            set missingWarn(x) {
                m = x,
                P.missingWarn = m
            },
            get fallbackWarn() {
                return p
            },
            set fallbackWarn(x) {
                p = x,
                P.fallbackWarn = p
            },
            get fallbackRoot() {
                return b
            },
            set fallbackRoot(x) {
                b = x
            },
            get fallbackFormat() {
                return E
            },
            set fallbackFormat(x) {
                E = x,
                P.fallbackFormat = E
            },
            get warnHtmlMessage() {
                return A
            },
            set warnHtmlMessage(x) {
                A = x,
                P.warnHtmlMessage = x
            },
            get escapeParameter() {
                return T
            },
            set escapeParameter(x) {
                T = x,
                P.escapeParameter = x
            },
            t: me,
            getLocaleMessage: M,
            setLocaleMessage: F,
            mergeLocaleMessage: z,
            getPostTranslationHandler: J,
            setPostTranslationHandler: te,
            getMissingHandler: Ie,
            setMissingHandler: ze,
            [Rd]: k
        };
        return U.datetimeFormats = ue,
        U.numberFormats = Z,
        U.rt = vt,
        U.te = w,
        U.tm = N,
        U.d = G,
        U.n = K,
        U.getDateTimeFormat = B,
        U.setDateTimeFormat = h,
        U.mergeDateTimeFormat = g,
        U.getNumberFormat = I,
        U.setNumberFormat = L,
        U.mergeNumberFormat = V,
        U[Ld] = i,
        U[Ho] = Ce,
        U[Ko] = on,
        U[qo] = Re,
        U
    }
    function rv(t) {
        const e = H(t.locale) ? t.locale : Di
          , n = H(t.fallbackLocale) || Te(t.fallbackLocale) || ne(t.fallbackLocale) || t.fallbackLocale === !1 ? t.fallbackLocale : e
          , i = $e(t.missing) ? t.missing : void 0
          , s = oe(t.silentTranslationWarn) || Rn(t.silentTranslationWarn) ? !t.silentTranslationWarn : !0
          , r = oe(t.silentFallbackWarn) || Rn(t.silentFallbackWarn) ? !t.silentFallbackWarn : !0
          , o = oe(t.fallbackRoot) ? t.fallbackRoot : !0
          , a = !!t.formatFallbackMessages
          , l = ne(t.modifiers) ? t.modifiers : {}
          , c = t.pluralizationRules
          , u = $e(t.postTranslation) ? t.postTranslation : void 0
          , f = H(t.warnHtmlInMessage) ? t.warnHtmlInMessage !== "off" : !0
          , d = !!t.escapeParameterHtml
          , _ = oe(t.sync) ? t.sync : !0;
        let m = t.messages;
        if (ne(t.sharedMessages)) {
            const T = t.sharedMessages;
            m = Object.keys(T).reduce( (C, P) => {
                const W = C[P] || (C[P] = {});
                return Ye(W, T[P]),
                C
            }
            , m || {})
        }
        const {__i18n: p, __root: b, __injectWithOption: E} = t
          , S = t.datetimeFormats
          , v = t.numberFormats
          , y = t.flatJson
          , A = t.translateExistCompatible;
        return {
            locale: e,
            fallbackLocale: n,
            messages: m,
            flatJson: y,
            datetimeFormats: S,
            numberFormats: v,
            missing: i,
            missingWarn: s,
            fallbackWarn: r,
            fallbackRoot: o,
            fallbackFormat: a,
            modifiers: l,
            pluralRules: c,
            postTranslation: u,
            warnHtmlMessage: f,
            escapeParameter: d,
            messageResolver: t.messageResolver,
            inheritLocale: _,
            translateExistCompatible: A,
            __i18n: p,
            __root: b,
            __injectWithOption: E
        }
    }
    function Yo(t={}, e) {
        {
            const n = Ba(rv(t))
              , {__extender: i} = t
              , s = {
                id: n.id,
                get locale() {
                    return n.locale.value
                },
                set locale(r) {
                    n.locale.value = r
                },
                get fallbackLocale() {
                    return n.fallbackLocale.value
                },
                set fallbackLocale(r) {
                    n.fallbackLocale.value = r
                },
                get messages() {
                    return n.messages.value
                },
                get datetimeFormats() {
                    return n.datetimeFormats.value
                },
                get numberFormats() {
                    return n.numberFormats.value
                },
                get availableLocales() {
                    return n.availableLocales
                },
                get formatter() {
                    return {
                        interpolate() {
                            return []
                        }
                    }
                },
                set formatter(r) {},
                get missing() {
                    return n.getMissingHandler()
                },
                set missing(r) {
                    n.setMissingHandler(r)
                },
                get silentTranslationWarn() {
                    return oe(n.missingWarn) ? !n.missingWarn : n.missingWarn
                },
                set silentTranslationWarn(r) {
                    n.missingWarn = oe(r) ? !r : r
                },
                get silentFallbackWarn() {
                    return oe(n.fallbackWarn) ? !n.fallbackWarn : n.fallbackWarn
                },
                set silentFallbackWarn(r) {
                    n.fallbackWarn = oe(r) ? !r : r
                },
                get modifiers() {
                    return n.modifiers
                },
                get formatFallbackMessages() {
                    return n.fallbackFormat
                },
                set formatFallbackMessages(r) {
                    n.fallbackFormat = r
                },
                get postTranslation() {
                    return n.getPostTranslationHandler()
                },
                set postTranslation(r) {
                    n.setPostTranslationHandler(r)
                },
                get sync() {
                    return n.inheritLocale
                },
                set sync(r) {
                    n.inheritLocale = r
                },
                get warnHtmlInMessage() {
                    return n.warnHtmlMessage ? "warn" : "off"
                },
                set warnHtmlInMessage(r) {
                    n.warnHtmlMessage = r !== "off"
                },
                get escapeParameterHtml() {
                    return n.escapeParameter
                },
                set escapeParameterHtml(r) {
                    n.escapeParameter = r
                },
                get preserveDirectiveContent() {
                    return !0
                },
                set preserveDirectiveContent(r) {},
                get pluralizationRules() {
                    return n.pluralRules || {}
                },
                __composer: n,
                t(...r) {
                    const [o,a,l] = r
                      , c = {};
                    let u = null
                      , f = null;
                    if (!H(o))
                        throw Ve(je.INVALID_ARGUMENT);
                    const d = o;
                    return H(a) ? c.locale = a : Te(a) ? u = a : ne(a) && (f = a),
                    Te(l) ? u = l : ne(l) && (f = l),
                    Reflect.apply(n.t, n, [d, u || f || {}, c])
                },
                rt(...r) {
                    return Reflect.apply(n.rt, n, [...r])
                },
                tc(...r) {
                    const [o,a,l] = r
                      , c = {
                        plural: 1
                    };
                    let u = null
                      , f = null;
                    if (!H(o))
                        throw Ve(je.INVALID_ARGUMENT);
                    const d = o;
                    return H(a) ? c.locale = a : De(a) ? c.plural = a : Te(a) ? u = a : ne(a) && (f = a),
                    H(l) ? c.locale = l : Te(l) ? u = l : ne(l) && (f = l),
                    Reflect.apply(n.t, n, [d, u || f || {}, c])
                },
                te(r, o) {
                    return n.te(r, o)
                },
                tm(r) {
                    return n.tm(r)
                },
                getLocaleMessage(r) {
                    return n.getLocaleMessage(r)
                },
                setLocaleMessage(r, o) {
                    n.setLocaleMessage(r, o)
                },
                mergeLocaleMessage(r, o) {
                    n.mergeLocaleMessage(r, o)
                },
                d(...r) {
                    return Reflect.apply(n.d, n, [...r])
                },
                getDateTimeFormat(r) {
                    return n.getDateTimeFormat(r)
                },
                setDateTimeFormat(r, o) {
                    n.setDateTimeFormat(r, o)
                },
                mergeDateTimeFormat(r, o) {
                    n.mergeDateTimeFormat(r, o)
                },
                n(...r) {
                    return Reflect.apply(n.n, n, [...r])
                },
                getNumberFormat(r) {
                    return n.getNumberFormat(r)
                },
                setNumberFormat(r, o) {
                    n.setNumberFormat(r, o)
                },
                mergeNumberFormat(r, o) {
                    n.mergeNumberFormat(r, o)
                },
                getChoiceIndex(r, o) {
                    return -1
                }
            };
            return s.__extender = i,
            s
        }
    }
    const za = {
        tag: {
            type: [String, Object]
        },
        locale: {
            type: String
        },
        scope: {
            type: String,
            validator: t => t === "parent" || t === "global",
            default: "parent"
        },
        i18n: {
            type: Object
        }
    };
    function ov({slots: t}, e) {
        return e.length === 1 && e[0] === "default" ? (t.default ? t.default() : []).reduce( (i, s) => [...i, ...s.type === Mt ? s.children : [s]], []) : e.reduce( (n, i) => {
            const s = t[i];
            return s && (n[i] = s()),
            n
        }
        , ye())
    }
    function Md(t) {
        return Mt
    }
    const av = fa({
        name: "i18n-t",
        props: Ye({
            keypath: {
                type: String,
                required: !0
            },
            plural: {
                type: [Number, String],
                validator: t => De(t) || !isNaN(t)
            }
        }, za),
        setup(t, e) {
            const {slots: n, attrs: i} = e
              , s = t.i18n || Rr({
                useScope: t.scope,
                __useComponent: !0
            });
            return () => {
                const r = Object.keys(n).filter(f => f !== "_")
                  , o = ye();
                t.locale && (o.locale = t.locale),
                t.plural !== void 0 && (o.plural = H(t.plural) ? +t.plural : t.plural);
                const a = ov(e, r)
                  , l = s[Ho](t.keypath, a, o)
                  , c = Ye(ye(), i)
                  , u = H(t.tag) || fe(t.tag) ? t.tag : Md();
                return ju(u, c, l)
            }
        }
    })
      , Sc = av;
    function lv(t) {
        return Te(t) && !H(t[0])
    }
    function Fd(t, e, n, i) {
        const {slots: s, attrs: r} = e;
        return () => {
            const o = {
                part: !0
            };
            let a = ye();
            t.locale && (o.locale = t.locale),
            H(t.format) ? o.key = t.format : fe(t.format) && (H(t.format.key) && (o.key = t.format.key),
            a = Object.keys(t.format).reduce( (d, _) => n.includes(_) ? Ye(ye(), d, {
                [_]: t.format[_]
            }) : d, ye()));
            const l = i(t.value, o, a);
            let c = [o.key];
            Te(l) ? c = l.map( (d, _) => {
                const m = s[d.type]
                  , p = m ? m({
                    [d.type]: d.value,
                    index: _,
                    parts: l
                }) : [d.value];
                return lv(p) && (p[0].key = "".concat(d.type, "-").concat(_)),
                p
            }
            ) : H(l) && (c = [l]);
            const u = Ye(ye(), r)
              , f = H(t.tag) || fe(t.tag) ? t.tag : Md();
            return ju(f, u, c)
        }
    }
    const cv = fa({
        name: "i18n-n",
        props: Ye({
            value: {
                type: Number,
                required: !0
            },
            format: {
                type: [String, Object]
            }
        }, za),
        setup(t, e) {
            const n = t.i18n || Rr({
                useScope: t.scope,
                __useComponent: !0
            });
            return Fd(t, e, Id, (...i) => n[qo](...i))
        }
    })
      , Oc = cv
      , uv = fa({
        name: "i18n-d",
        props: Ye({
            value: {
                type: [Number, Date],
                required: !0
            },
            format: {
                type: [String, Object]
            }
        }, za),
        setup(t, e) {
            const n = t.i18n || Rr({
                useScope: t.scope,
                __useComponent: !0
            });
            return Fd(t, e, xd, (...i) => n[Ko](...i))
        }
    })
      , Pc = uv;
    function fv(t, e) {
        const n = t;
        if (t.mode === "composition")
            return n.__getInstance(e) || t.global;
        {
            const i = n.__getInstance(e);
            return i != null ? i.__composer : t.global.__composer
        }
    }
    function dv(t) {
        const e = o => {
            const {instance: a, modifiers: l, value: c} = o;
            if (!a || !a.$)
                throw Ve(je.UNEXPECTED_ERROR);
            const u = fv(t, a.$)
              , f = xc(c);
            return [Reflect.apply(u.t, u, [...Ic(f)]), u]
        }
        ;
        return {
            created: (o, a) => {
                const [l,c] = e(a);
                fr && t.global === c && (o.__i18nWatcher = Sn(c.locale, () => {
                    a.instance && a.instance.$forceUpdate()
                }
                )),
                o.__composer = c,
                o.textContent = l
            }
            ,
            unmounted: o => {
                fr && o.__i18nWatcher && (o.__i18nWatcher(),
                o.__i18nWatcher = void 0,
                delete o.__i18nWatcher),
                o.__composer && (o.__composer = void 0,
                delete o.__composer)
            }
            ,
            beforeUpdate: (o, {value: a}) => {
                if (o.__composer) {
                    const l = o.__composer
                      , c = xc(a);
                    o.textContent = Reflect.apply(l.t, l, [...Ic(c)])
                }
            }
            ,
            getSSRProps: o => {
                const [a] = e(o);
                return {
                    textContent: a
                }
            }
        }
    }
    function xc(t) {
        if (H(t))
            return {
                path: t
            };
        if (ne(t)) {
            if (!("path"in t))
                throw Ve(je.REQUIRED_VALUE, "path");
            return t
        } else
            throw Ve(je.INVALID_VALUE)
    }
    function Ic(t) {
        const {path: e, locale: n, args: i, choice: s, plural: r} = t
          , o = {}
          , a = i || {};
        return H(n) && (o.locale = n),
        De(s) && (o.plural = s),
        De(r) && (o.plural = r),
        [e, a, o]
    }
    function pv(t, e, ...n) {
        const i = ne(n[0]) ? n[0] : {}
          , s = !!i.useI18nComponentName;
        (oe(i.globalInstall) ? i.globalInstall : !0) && ([s ? "i18n" : Sc.name, "I18nT"].forEach(o => t.component(o, Sc)),
        [Oc.name, "I18nN"].forEach(o => t.component(o, Oc)),
        [Pc.name, "I18nD"].forEach(o => t.component(o, Pc))),
        t.directive("t", dv(e))
    }
    function _v(t, e, n) {
        return {
            beforeCreate() {
                const i = fs();
                if (!i)
                    throw Ve(je.UNEXPECTED_ERROR);
                const s = this.$options;
                if (s.i18n) {
                    const r = s.i18n;
                    if (s.__i18n && (r.__i18n = s.__i18n),
                    r.__root = e,
                    this === this.$root)
                        this.$i18n = Cc(t, r);
                    else {
                        r.__injectWithOption = !0,
                        r.__extender = n.__vueI18nExtend,
                        this.$i18n = Yo(r);
                        const o = this.$i18n;
                        o.__extender && (o.__disposer = o.__extender(this.$i18n))
                    }
                } else if (s.__i18n)
                    if (this === this.$root)
                        this.$i18n = Cc(t, s);
                    else {
                        this.$i18n = Yo({
                            __i18n: s.__i18n,
                            __injectWithOption: !0,
                            __extender: n.__vueI18nExtend,
                            __root: e
                        });
                        const r = this.$i18n;
                        r.__extender && (r.__disposer = r.__extender(this.$i18n))
                    }
                else
                    this.$i18n = t;
                s.__i18nGlobal && Dd(e, s, s),
                this.$t = (...r) => this.$i18n.t(...r),
                this.$rt = (...r) => this.$i18n.rt(...r),
                this.$tc = (...r) => this.$i18n.tc(...r),
                this.$te = (r, o) => this.$i18n.te(r, o),
                this.$d = (...r) => this.$i18n.d(...r),
                this.$n = (...r) => this.$i18n.n(...r),
                this.$tm = r => this.$i18n.tm(r),
                n.__setInstance(i, this.$i18n)
            },
            mounted() {},
            unmounted() {
                const i = fs();
                if (!i)
                    throw Ve(je.UNEXPECTED_ERROR);
                const s = this.$i18n;
                delete this.$t,
                delete this.$rt,
                delete this.$tc,
                delete this.$te,
                delete this.$d,
                delete this.$n,
                delete this.$tm,
                s.__disposer && (s.__disposer(),
                delete s.__disposer,
                delete s.__extender),
                n.__deleteInstance(i),
                delete this.$i18n
            }
        }
    }
    function Cc(t, e) {
        t.locale = e.locale || t.locale,
        t.fallbackLocale = e.fallbackLocale || t.fallbackLocale,
        t.missing = e.missing || t.missing,
        t.silentTranslationWarn = e.silentTranslationWarn || t.silentFallbackWarn,
        t.silentFallbackWarn = e.silentFallbackWarn || t.silentFallbackWarn,
        t.formatFallbackMessages = e.formatFallbackMessages || t.formatFallbackMessages,
        t.postTranslation = e.postTranslation || t.postTranslation,
        t.warnHtmlInMessage = e.warnHtmlInMessage || t.warnHtmlInMessage,
        t.escapeParameterHtml = e.escapeParameterHtml || t.escapeParameterHtml,
        t.sync = e.sync || t.sync,
        t.__composer[Rd](e.pluralizationRules || t.pluralizationRules);
        const n = Cr(t.locale, {
            messages: e.messages,
            __i18n: e.__i18n
        });
        return Object.keys(n).forEach(i => t.mergeLocaleMessage(i, n[i])),
        e.datetimeFormats && Object.keys(e.datetimeFormats).forEach(i => t.mergeDateTimeFormat(i, e.datetimeFormats[i])),
        e.numberFormats && Object.keys(e.numberFormats).forEach(i => t.mergeNumberFormat(i, e.numberFormats[i])),
        t
    }
    const hv = Fn("global-vue-i18n");
    function mv(t={}, e) {
        const n = __VUE_I18N_LEGACY_API__ && oe(t.legacy) ? t.legacy : __VUE_I18N_LEGACY_API__
          , i = oe(t.globalInjection) ? t.globalInjection : !0
          , s = __VUE_I18N_LEGACY_API__ && n ? !!t.allowComposition : !0
          , r = new Map
          , [o,a] = gv(t, n)
          , l = Fn("");
        function c(d) {
            return r.get(d) || null
        }
        function u(d, _) {
            r.set(d, _)
        }
        function f(d) {
            r.delete(d)
        }
        {
            let _;
            const d = {
                get mode() {
                    return __VUE_I18N_LEGACY_API__ && n ? "legacy" : "composition"
                },
                get allowComposition() {
                    return s
                },
                install(m, ...p) {
                    return At(this, null, function*() {
                        if (m.__VUE_I18N_SYMBOL__ = l,
                        m.provide(m.__VUE_I18N_SYMBOL__, d),
                        ne(p[0])) {
                            const S = p[0];
                            d.__composerExtend = S.__composerExtend,
                            d.__vueI18nExtend = S.__vueI18nExtend
                        }
                        let b = null;
                        !n && i && (b = wv(m, d.global)),
                        __VUE_I18N_FULL_INSTALL__ && pv(m, d, ...p),
                        __VUE_I18N_LEGACY_API__ && n && m.mixin(_v(a, a.__composer, d));
                        const E = m.unmount;
                        m.unmount = () => {
                            b && b(),
                            d.dispose(),
                            E()
                        }
                    })
                },
                get global() {
                    return a
                },
                dispose() {
                    o.stop()
                },
                __instances: r,
                __getInstance: c,
                __setInstance: u,
                __deleteInstance: f
            };
            return d
        }
    }
    function Rr(t={}) {
        const e = fs();
        if (e == null)
            throw Ve(je.MUST_BE_CALL_SETUP_TOP);
        if (!e.isCE && e.appContext.app != null && !e.appContext.app.__VUE_I18N_SYMBOL__)
            throw Ve(je.NOT_INSTALLED);
        const n = bv(e)
          , i = vv(n)
          , s = Nd(e)
          , r = yv(t, s);
        if (__VUE_I18N_LEGACY_API__ && n.mode === "legacy" && !t.__useComponent) {
            if (!n.allowComposition)
                throw Ve(je.NOT_AVAILABLE_IN_LEGACY_MODE);
            return Tv(e, r, i, t)
        }
        if (r === "global")
            return Dd(i, t, s),
            i;
        if (r === "parent") {
            let l = kv(n, e, t.__useComponent);
            return l == null && (l = i),
            l
        }
        const o = n;
        let a = o.__getInstance(e);
        if (a == null) {
            const l = Ye({}, t);
            "__i18n"in s && (l.__i18n = s.__i18n),
            i && (l.__root = i),
            a = Ba(l),
            o.__composerExtend && (a[Wo] = o.__composerExtend(a)),
            $v(o, e, a),
            o.__setInstance(e, a)
        }
        return a
    }
    function gv(t, e, n) {
        const i = rp();
        {
            const s = __VUE_I18N_LEGACY_API__ && e ? i.run( () => Yo(t)) : i.run( () => Ba(t));
            if (s == null)
                throw Ve(je.UNEXPECTED_ERROR);
            return [i, s]
        }
    }
    function bv(t) {
        {
            const e = ts(t.isCE ? hv : t.appContext.app.__VUE_I18N_SYMBOL__);
            if (!e)
                throw Ve(t.isCE ? je.NOT_INSTALLED_WITH_PROVIDE : je.UNEXPECTED_ERROR);
            return e
        }
    }
    function yv(t, e) {
        return Pr(t) ? "__i18n"in e ? "local" : "global" : t.useScope ? t.useScope : "local"
    }
    function vv(t) {
        return t.mode === "composition" ? t.global : t.global.__composer
    }
    function kv(t, e, n=!1) {
        let i = null;
        const s = e.root;
        let r = Av(e, n);
        for (; r != null; ) {
            const o = t;
            if (t.mode === "composition")
                i = o.__getInstance(r);
            else if (__VUE_I18N_LEGACY_API__) {
                const a = o.__getInstance(r);
                a != null && (i = a.__composer,
                n && i && !i[Ld] && (i = null))
            }
            if (i != null || s === r)
                break;
            r = r.parent
        }
        return i
    }
    function Av(t, e=!1) {
        return t == null ? null : e && t.vnode.ctx || t.parent
    }
    function $v(t, e, n) {
        da( () => {}
        , e),
        pa( () => {
            const i = n;
            t.__deleteInstance(e);
            const s = i[Wo];
            s && (s(),
            delete i[Wo])
        }
        , e)
    }
    function Tv(t, e, n, i={}) {
        const s = e === "local"
          , r = ru(null);
        if (s && t.proxy && !(t.proxy.$options.i18n || t.proxy.$options.__i18n))
            throw Ve(je.MUST_DEFINE_I18N_OPTION_IN_ALLOW_COMPOSITION);
        const o = oe(i.inheritLocale) ? i.inheritLocale : !H(i.locale)
          , a = Me(!s || o ? n.locale.value : H(i.locale) ? i.locale : Di)
          , l = Me(!s || o ? n.fallbackLocale.value : H(i.fallbackLocale) || Te(i.fallbackLocale) || ne(i.fallbackLocale) || i.fallbackLocale === !1 ? i.fallbackLocale : a.value)
          , c = Me(Cr(a.value, i))
          , u = Me(ne(i.datetimeFormats) ? i.datetimeFormats : {
            [a.value]: {}
        })
          , f = Me(ne(i.numberFormats) ? i.numberFormats : {
            [a.value]: {}
        })
          , d = s ? n.missingWarn : oe(i.missingWarn) || Rn(i.missingWarn) ? i.missingWarn : !0
          , _ = s ? n.fallbackWarn : oe(i.fallbackWarn) || Rn(i.fallbackWarn) ? i.fallbackWarn : !0
          , m = s ? n.fallbackRoot : oe(i.fallbackRoot) ? i.fallbackRoot : !0
          , p = !!i.fallbackFormat
          , b = $e(i.missing) ? i.missing : null
          , E = $e(i.postTranslation) ? i.postTranslation : null
          , S = s ? n.warnHtmlMessage : oe(i.warnHtmlMessage) ? i.warnHtmlMessage : !0
          , v = !!i.escapeParameter
          , y = s ? n.modifiers : ne(i.modifiers) ? i.modifiers : {}
          , A = i.pluralRules || s && n.pluralRules;
        function T() {
            return [a.value, l.value, c.value, u.value, f.value]
        }
        const O = Ft({
            get: () => r.value ? r.value.locale.value : a.value,
            set: $ => {
                r.value && (r.value.locale.value = $),
                a.value = $
            }
        })
          , C = Ft({
            get: () => r.value ? r.value.fallbackLocale.value : l.value,
            set: $ => {
                r.value && (r.value.fallbackLocale.value = $),
                l.value = $
            }
        })
          , P = Ft( () => r.value ? r.value.messages.value : c.value)
          , W = Ft( () => u.value)
          , X = Ft( () => f.value);
        function q() {
            return r.value ? r.value.getPostTranslationHandler() : E
        }
        function Q($) {
            r.value && r.value.setPostTranslationHandler($)
        }
        function _e() {
            return r.value ? r.value.getMissingHandler() : b
        }
        function ue($) {
            r.value && r.value.setMissingHandler($)
        }
        function Z($) {
            return T(),
            $()
        }
        function J(...$) {
            return r.value ? Z( () => Reflect.apply(r.value.t, null, [...$])) : Z( () => "")
        }
        function te(...$) {
            return r.value ? Reflect.apply(r.value.rt, null, [...$]) : ""
        }
        function Ie(...$) {
            return r.value ? Z( () => Reflect.apply(r.value.d, null, [...$])) : Z( () => "")
        }
        function ze(...$) {
            return r.value ? Z( () => Reflect.apply(r.value.n, null, [...$])) : Z( () => "")
        }
        function he($) {
            return r.value ? r.value.tm($) : {}
        }
        function me($, N) {
            return r.value ? r.value.te($, N) : !1
        }
        function vt($) {
            return r.value ? r.value.getLocaleMessage($) : {}
        }
        function G($, N) {
            r.value && (r.value.setLocaleMessage($, N),
            c.value[$] = N)
        }
        function K($, N) {
            r.value && r.value.mergeLocaleMessage($, N)
        }
        function ge($) {
            return r.value ? r.value.getDateTimeFormat($) : {}
        }
        function be($, N) {
            r.value && (r.value.setDateTimeFormat($, N),
            u.value[$] = N)
        }
        function Ue($, N) {
            r.value && r.value.mergeDateTimeFormat($, N)
        }
        function Ce($) {
            return r.value ? r.value.getNumberFormat($) : {}
        }
        function Re($, N) {
            r.value && (r.value.setNumberFormat($, N),
            f.value[$] = N)
        }
        function on($, N) {
            r.value && r.value.mergeNumberFormat($, N)
        }
        const k = {
            get id() {
                return r.value ? r.value.id : -1
            },
            locale: O,
            fallbackLocale: C,
            messages: P,
            datetimeFormats: W,
            numberFormats: X,
            get inheritLocale() {
                return r.value ? r.value.inheritLocale : o
            },
            set inheritLocale($) {
                r.value && (r.value.inheritLocale = $)
            },
            get availableLocales() {
                return r.value ? r.value.availableLocales : Object.keys(c.value)
            },
            get modifiers() {
                return r.value ? r.value.modifiers : y
            },
            get pluralRules() {
                return r.value ? r.value.pluralRules : A
            },
            get isGlobal() {
                return r.value ? r.value.isGlobal : !1
            },
            get missingWarn() {
                return r.value ? r.value.missingWarn : d
            },
            set missingWarn($) {
                r.value && (r.value.missingWarn = $)
            },
            get fallbackWarn() {
                return r.value ? r.value.fallbackWarn : _
            },
            set fallbackWarn($) {
                r.value && (r.value.missingWarn = $)
            },
            get fallbackRoot() {
                return r.value ? r.value.fallbackRoot : m
            },
            set fallbackRoot($) {
                r.value && (r.value.fallbackRoot = $)
            },
            get fallbackFormat() {
                return r.value ? r.value.fallbackFormat : p
            },
            set fallbackFormat($) {
                r.value && (r.value.fallbackFormat = $)
            },
            get warnHtmlMessage() {
                return r.value ? r.value.warnHtmlMessage : S
            },
            set warnHtmlMessage($) {
                r.value && (r.value.warnHtmlMessage = $)
            },
            get escapeParameter() {
                return r.value ? r.value.escapeParameter : v
            },
            set escapeParameter($) {
                r.value && (r.value.escapeParameter = $)
            },
            t: J,
            getPostTranslationHandler: q,
            setPostTranslationHandler: Q,
            getMissingHandler: _e,
            setMissingHandler: ue,
            rt: te,
            d: Ie,
            n: ze,
            tm: he,
            te: me,
            getLocaleMessage: vt,
            setLocaleMessage: G,
            mergeLocaleMessage: K,
            getDateTimeFormat: ge,
            setDateTimeFormat: be,
            mergeDateTimeFormat: Ue,
            getNumberFormat: Ce,
            setNumberFormat: Re,
            mergeNumberFormat: on
        };
        function w($) {
            $.locale.value = a.value,
            $.fallbackLocale.value = l.value,
            Object.keys(c.value).forEach(N => {
                $.mergeLocaleMessage(N, c.value[N])
            }
            ),
            Object.keys(u.value).forEach(N => {
                $.mergeDateTimeFormat(N, u.value[N])
            }
            ),
            Object.keys(f.value).forEach(N => {
                $.mergeNumberFormat(N, f.value[N])
            }
            ),
            $.escapeParameter = v,
            $.fallbackFormat = p,
            $.fallbackRoot = m,
            $.fallbackWarn = _,
            $.missingWarn = d,
            $.warnHtmlMessage = S
        }
        return gu( () => {
            if (t.proxy == null || t.proxy.$i18n == null)
                throw Ve(je.NOT_AVAILABLE_COMPOSITION_IN_LEGACY);
            const $ = r.value = t.proxy.$i18n.__composer;
            e === "global" ? (a.value = $.locale.value,
            l.value = $.fallbackLocale.value,
            c.value = $.messages.value,
            u.value = $.datetimeFormats.value,
            f.value = $.numberFormats.value) : s && w($)
        }
        ),
        k
    }
    const Ev = ["locale", "fallbackLocale", "availableLocales"]
      , Rc = ["t", "rt", "d", "n", "tm", "te"];
    function wv(t, e) {
        const n = Object.create(null);
        return Ev.forEach(s => {
            const r = Object.getOwnPropertyDescriptor(e, s);
            if (!r)
                throw Ve(je.UNEXPECTED_ERROR);
            const o = qe(r.value) ? {
                get() {
                    return r.value.value
                },
                set(a) {
                    r.value.value = a
                }
            } : {
                get() {
                    return r.get && r.get()
                }
            };
            Object.defineProperty(n, s, o)
        }
        ),
        t.config.globalProperties.$i18n = n,
        Rc.forEach(s => {
            const r = Object.getOwnPropertyDescriptor(e, s);
            if (!r || !r.value)
                throw Ve(je.UNEXPECTED_ERROR);
            Object.defineProperty(t.config.globalProperties, "$".concat(s), r)
        }
        ),
        () => {
            delete t.config.globalProperties.$i18n,
            Rc.forEach(s => {
                delete t.config.globalProperties["$".concat(s)]
            }
            )
        }
    }
    tv();
    __INTLIFY_JIT_COMPILATION__ ? dc(Yy) : dc(Wy);
    Ey(ry);
    wy(vd);
    if (__INTLIFY_PROD_DEVTOOLS__) {
        const t = hn();
        t.__INTLIFY__ = !0,
        _y(t.__INTLIFY_DEVTOOLS_GLOBAL_HOOK__)
    }
    const Sv = {
        class: "mainContent"
    }
      , Ov = {
        class: "content"
    }
      , Pv = {
        class: "pointer"
    }
      , xv = {
        class: "watermark"
    }
      , Iv = {
        class: "holder"
    }
      , Cv = {
        class: "checktext"
    }
      , Rv = {
        class: "header"
    }
      , Lv = {
        key: 0,
        class: "logo"
    }
      , Nv = {
        key: 1,
        class: "logoContainer"
    }
      , Dv = ["innerHTML"]
      , Mv = {
        key: 1,
        class: "success"
    }
      , Fv = {
        class: "loadGlowImage"
    }
      , Gv = {
        class: "statusImage"
    }
      , Uv = {
        class: "shieldImage"
    }
      , jv = {
        key: 0
    }
      , Bv = {
        key: 1
    }
      , zv = {
        class: "text"
    }
      , Vv = {
        class: "loadScaleImage"
    }
      , Hv = ["src"]
      , Kv = {
        key: 3,
        class: "fail"
    }
      , qv = {
        key: 0
    }
      , Wv = {
        key: 1
    }
      , Yv = ["innerHTML"]
      , Xv = {
        key: 2
    }
      , Jv = ["innerHTML"]
      , Qv = {
        key: 4,
        class: "load"
    }
      , Zv = "AR FA UR"
      , ek = {
        __name: "App",
        setup(t) {
            const e = Me("")
              , n = Me("load")
              , i = Me(0);
            let s = Me({
                width: 414,
                height: 736
            });
            const r = Me(fb.version)
              , {t: o, locale: a, messages: l, availableLocales: c} = Rr()
              , u = window.location.origin;
            var f = "api.staging.pgsoft.com";
            const d = Me("");
            var _ = {
                attempts: 0,
                max: 60,
                time: 1500,
                timeout: null
            };
            const m = Me(!1)
              , p = Me(!1)
              , b = Me(!0);
            var E = "";
            const S = Me(!0)
              , v = Me(!1)
              , y = Me(!1);
            da( () => {
                if (window.addEventListener("resize", W),
                window.addEventListener("message", Q),
                window.parent.postMessage({
                    type: "pg.web-history.redirect.Check"
                }, "*"),
                !/log=true/.test(window.location))
                    A();
                else {
                    var G = document.createElement("script");
                    G.src = "//cdn.jsdelivr.net/npm/eruda",
                    document.body.appendChild(G),
                    G.onload = function() {
                        eruda.init({
                            tool: ["console", "elements", "network", "info", "sources", "resource"]
                        }),
                        eruda.position({
                            x: -1,
                            y: -1
                        }),
                        A()
                    }
                }
            }
            );
            function A(G) {
                vt();
                const K = new URLSearchParams(window.location.search);
                if (K.get("l") !== null && (a.value = K.get("l").toUpperCase()),
                K.get("env") && K.get("sid") && K.get("gid") && K.get("atk"))
                    C(K);
                else if (n.value = "wrong",
                K.get("thai") === "true" && (b.value = !0),
                K.get("ui-test") && (n.value = K.get("ui-test")),
                K.get("l") === "loop") {
                    let ge = ["en", "zh"];
                    for (let be in l.value) {
                        let Ue = be.toLocaleLowerCase();
                        Ue !== "en" && Ue !== "zh" && ge.push(Ue)
                    }
                    console.warn(ge.length, ge.join(",")),
                    window.addEventListener("keydown", be => {
                        be.key == "ArrowRight" ? (T += 1,
                        T >= ge.length - 1 && (T = 0),
                        a.value = ge[T].toUpperCase(),
                        console.warn(T + "." + ge[T].toUpperCase())) : be.key == "ArrowLeft" && (T -= 1,
                        T < 0 && (T = ge.length - 1),
                        a.value = ge[T].toUpperCase(),
                        console.warn(T + "." + ge[T].toUpperCase()))
                    }
                    )
                }
            }
            let T = 0;
            Sn(n, G => At(this, null, function*() {
                switch (G) {
                case "success":
                    d.value = "success",
                    E || P("bypass"),
                    X = !0;
                    break;
                case "nofound":
                    d.value = "failed";
                    break;
                case "fake":
                    d.value = "failed";
                    break;
                case "error":
                    d.value = "error";
                    break;
                case "expired":
                    d.value = "error";
                    break;
                case "wrong":
                    d.value = "error";
                    break;
                case "load":
                    d.value = "",
                    e.value = "",
                    X = !1;
                    break;
                default:
                    e.value = "",
                    X = !1
                }
            })),
            Sn(a, G => At(this, null, function*() {
                Zv.indexOf(G) >= 0 ? m.value = !0 : m.value = !1
            }));
            function O(G=null) {
                switch (G) {
                case "5000":
                    n.value = "nofound";
                    break;
                case "2125":
                    n.value = "fake";
                    break;
                case "1302":
                case "1308":
                    n.value = "expired";
                    break;
                case "1307":
                case "1800":
                case "2122":
                    n.value = "wrong";
                    break;
                case "1003":
                    n.value = "error";
                    break;
                default:
                    n.value = "error"
                }
            }
            function C(G) {
                window.location.port || (f = window.location.hostname.replace("verify", "api"));
                let K = "";
                G.get("trace_id") && (K = "?trace_id=".concat(G.get("trace_id")));
                let ge = "https://".concat(f, "/AuthenticationVerify/GetBetHistoryVerifyHtml");
                // let ge = "https://localhost:7209/AuthenticationVerify/GetBetHistoryVerifyHtml";
                
                Pe.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded",
                Pe.defaults.withCredentials = !0;
                let be = {
                    type: "verify",
                    sid: "".concat(G.get("sid") || "1806217391126872576"),
                    gid: "".concat(G.get("gid") || "1738001"),
                    atk: "".concat(G.get("atk") || "BD139AD5-C341-4F32-9569-BD4AC6D6A515"),
                    l: "".concat(G.get("l") || "")
                }
                  , Ue = [];
                for (const Ce in be)
                    be[Ce] !== "" && Ue.push("".concat(Ce, "=").concat(be[Ce]));
                Pe.post("".concat(ge).concat(K), {
                    ea: Ue.join("&"),
                    env: G.get("env")
                }).then(Ce => At(this, null, function*() {
                    Ce.data.dt ? (E = Ce.data.dt.content,
                    P(Ce.data.dt.content)) : O(Ce.data.err.cd || null)
                })).catch(Ce => {
                    _.attempts <= _.max ? (_.attempts += 1,
                    _.timeout = setTimeout(C.bind(null, G), _.time)) : n.value = "error"
                }
                )
            }
            function P(G) {
                return At(this, null, function*() {
                    let K = document.querySelector("#iframegame");
                    e.value = "about:blank";
                    let ge = G;
                    if (G.length <= 50) {
                        let be = 'style="background: #303036; color:#FAD171; font-size:15px; font-family:Arial; word-wrap: break-word;';
                        be += 'height: 100%; display: flex; align-items: center; justify-content: center;"',
                        ge = "<div ".concat(be, ">").concat(G, "<br>")
                    }
                    yield cu(),
                    setTimeout( () => {
                        try {
                            if (K.contentWindow.document.open(),
                            K.contentWindow.document.write(ge),
                            K.contentWindow.document.close(),
                            !p.value)
                                console.warn("Not Support postMessage"),
                                setTimeout( () => {
                                    X = !0,
                                    ue()
                                }
                                , 5e3);
                            else {
                                let be = 38;
                                setTimeout( () => {
                                    n.value === "load" && (console.warn("No postMessage received after ".concat(be, "secs")),
                                    X = !0,
                                    ue())
                                }
                                , be * 1e3)
                            }
                        } catch (be) {
                            P("Write doc fails!")
                        }
                    }
                    , 0)
                })
            }
            function W() {
                if (e.value !== "" && n.value === "success") {
                    let G = document.querySelector(".iframeContainer").getBoundingClientRect().width;
                    i.value = Math.round(G / s.value.width * 100) / 100
                }
            }
            var X = !1
              , q = !1;
            function Q(G) {
                G.data.type && G.data.type.indexOf("redirect.") >= 0 && (G.data.type.indexOf("Check") >= 0 ? p.value = !0 : G.data.type.indexOf("Success") >= 0 ? (X = !0,
                ue()) : O(G.data.msg || null))
            }
            function _e() {
                q = !0,
                ue()
            }
            function ue() {
                return At(this, null, function*() {
                    X && q && (n.value = "success",
                    setTimeout(ze, 100),
                    document.querySelector(".loadScaleImage").remove(),
                    W(),
                    qs.from(".iframeContainer", {
                        opacity: 0,
                        duration: .5,
                        delay: .3
                    }))
                })
            }
            var Z = !1;
            function J(G) {
                Z && G !== null && document.querySelector(".scaleSprite") && document.querySelector(".loadGlowImage") && (Ie.value = "00000",
                qs.from(".glowSprite", {
                    duration: .5,
                    opacity: 0,
                    delay: 0
                }),
                qs.to(".scaleSprite", {
                    duration: .5,
                    opacity: 0,
                    delay: .5,
                    onComplete: () => {
                        document.querySelector(".scaleSprite").remove(),
                        document.querySelector(".loadGlowImage").remove(),
                        he()
                    }
                }))
            }
            const te = Me("")
              , Ie = Me("");
            function ze() {
                let G = 0
                  , K = setInterval( () => {
                    te.value = G < 10 ? "0000".concat(G) : "000".concat(G),
                    G++,
                    G > 19 && (clearInterval(K),
                    Z = !0,
                    J())
                }
                , 55)
            }
            function he() {
                let G = 0
                  , K = setInterval( () => {
                    Ie.value = G < 10 ? "0000".concat(G) : "000".concat(G),
                    G++,
                    G > 23 && (clearInterval(K),
                    setTimeout(he, 2500))
                }
                , 55)
            }
            function me() {
                const G = window.innerHeight
                  , K = document.documentElement.clientHeight;
                return G > K
            }
            function vt() {
                var be, Ue;
                let G = navigator.userAgent || navigator.vendor || window.opera
                  , K = /iPad|Tablet|Nexus 7|Nexus 10|Galaxy Tab|Kindle/i.test(G);
                S.value = /Mobi|Android|iPhone|iPod|Windows Phone/i.test(G) && !K,
                G.includes("Safari") && !G.includes("Chrome") && !G.includes("CriOS") && G.includes("Android");
                let ge = /Mac|iPhone|iPad|iPod/i.test(G);
                if (S.value) {
                    if (ge && !K) {
                        v.value = !0;
                        let Re = ((Ue = (be = /iP(hone|ad|od) OS ([0-9_]+)/i.exec(window.navigator.userAgent)) == null ? void 0 : be[2]) == null ? void 0 : Ue.replace("_", ".")) || NaN;
                        isNaN(Re) || parseInt(Re.split(".")[0], 10) < 15 && (v.value = !1)
                    } else
                        v.value = !1;
                    let Ce = 0;
                    window.addEventListener("scroll", () => {
                        let Re = window.pageYOffset || document.documentElement.scrollTop
                          , on = document.documentElement.scrollHeight - document.documentElement.clientHeight;
                        !v.value && Re <= 100 || v.value && Re >= on - 100 ? y.value = !0 : Re > Ce ? y.value = !1 : v.value && Re < Ce - 10 ? y.value = !0 : me() ? y.value = !1 : y.value = !0,
                        Ce = Re <= 0 ? 0 : Re
                    }
                    ),
                    y.value = !0
                }
            }
            return (G, K) => (ot(),
            dt("div", Sv, [j("div", null, [n.value != "load" ? (ot(),
            dt("div", {
                key: 0,
                class: ce(["backg", [{
                    successBg: d.value === "success",
                    failedBg: d.value === "failed",
                    wrongBg: d.value === "error"
                }]])
            }, null, 2)) : ln("", !0), j("div", Ov, [n.value === "success" ? (ot(),
            dt("div", {
                key: 0,
                class: ce(["urlcheck", {
                    fixed: S.value,
                    bottom: v.value,
                    hide: !y.value
                }])
            }, [j("div", null, [j("div", Pv, [K[1] || (K[1] = j("div", {
                class: "left"
            }, [j("img", {
                src: kl
            })], -1)), j("div", {
                class: ce(["right", [{
                    max: b.value
                }]])
            }, K[0] || (K[0] = [j("img", {
                src: kl
            }, null, -1)]), 2)]), j("div", xv, [j("div", Iv, [j("div", {
                class: ce(["left", [{
                    th: b.value
                }]])
            }, null, 2), K[2] || (K[2] = j("div", {
                class: "mid"
            }, [j("div")], -1))])]), j("div", Cv, [j("div", {
                class: ce([{
                    rtl: m.value
                }])
            }, Xe(G.$t("chars")), 3)])])], 2)) : ln("", !0), j("div", Rv, [j("div", {
                class: ce(["top", [{
                    topth: b.value
                }]])
            }, [b.value ? ln("", !0) : (ot(),
            dt("div", Lv, K[3] || (K[3] = [j("img", {
                src: Al,
                alt: "Logo"
            }, null, -1)]))), b.value ? (ot(),
            dt("div", Nv, K[4] || (K[4] = [P_('<div class="logoth"><img src="' + Al + '" alt="Logo"></div><div class="cross"><div></div><div></div></div><div class="askmebet"><img src="' + fh + '" alt="askmebet"></div>', 3)]))) : ln("", !0), j("div", {
                class: ce(["line", [{
                    lineth: b.value
                }]])
            }, null, 2), j("div", {
                class: ce(["license", [{
                    licenseth: b.value
                }]])
            }, [j("div", {
                class: ce({
                    right: m.value
                })
            }, [j("div", {
                class: ce(["gtext", {
                    rtl: m.value
                }])
            }, Xe(G.$t("licensed_top")), 3), j("div", {
                class: ce(["logos", {
                    right: m.value
                }])
            }, K[5] || (K[5] = [j("div", {
                class: "gambling"
            }, [j("img", {
                src: dh
            })], -1), j("div", {
                class: "mga"
            }, [j("img", {
                src: ph
            })], -1)]), 2)], 2), j("div", {
                class: ce({
                    right: m.value
                })
            }, [j("div", {
                class: ce(["gtext", {
                    rtl: m.value
                }])
            }, Xe(G.$t("certified_top")), 3), j("div", {
                class: ce(["logos", {
                    right: m.value
                }])
            }, K[6] || (K[6] = [j("div", {
                class: "ga"
            }, [j("img", {
                src: _h
            })], -1), j("div", {
                class: "bmm"
            }, [j("img", {
                src: hh
            })], -1)]), 2)], 2)], 2)], 2), j("div", {
                class: ce(["certified", {
                    rtl: m.value
                }]),
                innerHTML: G.$t("certified")
            }, null, 10, Dv)]), n.value === "success" ? (ot(),
            dt("div", Mv, [j("div", Fv, [j("img", {
                src: mh,
                onLoad: J
            }, null, 32)]), j("div", Gv, [j("div", Uv, [j("span", {
                class: ce("shield_success_pop_up_".concat(te.value, " scaleSprite"))
            }, null, 2), j("span", {
                class: ce("shield_success_shiny_effect_".concat(Ie.value, " glowSprite"))
            }, null, 2)])]), j("div", {
                class: ce(["official", {
                    rtl: m.value
                }])
            }, [b.value ? (ot(),
            dt("span", Bv, [Fu(Xe(G.$t("official_th")) + " ", 1), j("div", zv, Xe(G.$t("th_only")), 1)])) : (ot(),
            dt("span", jv, Xe(G.$t("official")), 1))], 2)])) : ln("", !0), j("div", Vv, [j("img", {
                src: gh,
                onLoad: _e
            }, null, 32)]), n.value === "success" || n.value === "load" ? (ot(),
            dt("div", {
                key: 2,
                class: ce(["iframeContainer", i.value === 0 ? "" : "border"]),
                style: os({
                    height: hi(s).height * i.value + "px"
                })
            }, [j("div", {
                style: os({
                    transform: "scale(".concat(i.value, ")")
                })
            }, [j("iframe", {
                src: e.value,
                id: "iframegame",
                class: "iframeContent"
            }, null, 8, Hv)], 4)], 6)) : ln("", !0), n.value !== "success" && n.value !== "load" ? (ot(),
            dt("div", Kv, [n.value === "expired" || n.value === "wrong" ? (ot(),
            dt("div", qv, [j("div", null, [K[7] || (K[7] = j("div", {
                class: "errorLogo"
            }, [j("img", {
                src: bh
            })], -1)), j("div", {
                class: ce(["bigText blue", {
                    rtl: m.value
                }])
            }, Xe(G.$t("incompleted")), 3)]), j("div", {
                class: ce(["text blue incompletedReturn", {
                    rtl: m.value
                }])
            }, Xe(G.$t("incompleted_return")), 3), j("div", {
                class: ce(["text lineContainer", {
                    rtl: m.value
                }])
            }, [K[8] || (K[8] = j("div", {
                class: "line"
            }, null, -1)), j("div", null, Xe(G.$t("incompleted_or")), 1), K[9] || (K[9] = j("div", {
                class: "line"
            }, null, -1))], 2), j("div", {
                class: ce(["text blue incompletedHistory", {
                    rtl: m.value
                }]),
                style: {}
            }, Xe(G.$t("incompleted_history")), 3)])) : n.value === "fake" || n.value === "nofound" ? (ot(),
            dt("div", Wv, [j("div", null, [K[10] || (K[10] = j("div", {
                class: "errorLogo"
            }, [j("img", {
                src: yh
            })], -1)), j("div", {
                class: ce(["bigText red", {
                    rtl: m.value
                }])
            }, Xe(G.$t("fake")), 3)]), j("div", {
                class: ce(["text red", {
                    rtl: m.value
                }]),
                innerHTML: hi(o)("fake_details", {
                    link: "<span>".concat(hi(u), "</span>")
                })
            }, null, 10, Yv)])) : n.value === "error" ? (ot(),
            dt("div", Xv, [j("div", null, [K[11] || (K[11] = j("div", {
                class: "cloudLogo"
            }, [j("img", {
                src: vh
            })], -1)), j("div", {
                class: ce(["bigText blue", {
                    rtl: m.value
                }])
            }, Xe(hi(o)("unstable")), 3)]), j("div", {
                class: ce(["text blue", {
                    rtl: m.value
                }])
            }, Xe(G.$t("unstable_details")), 3)])) : ln("", !0), j("div", {
                class: ce(["warn", {
                    blue: d.value === "error",
                    red: d.value === "failed"
                }])
            }, [j("div", {
                class: ce({
                    rtl: m.value
                })
            }, [j("span", {
                class: ce({
                    blue: d.value === "error",
                    red: d.value === "failed"
                })
            }, Xe(G.$t("gentle")), 3), K[12] || (K[12] = j("span", {
                class: "line"
            }, null, -1)), j("span", {
                innerHTML: G.$t("reminder", {
                    link: "<span class='white'>".concat(hi(u), "</span>")
                })
            }, null, 8, Jv)], 2)], 2)])) : ln("", !0), n.value === "load" ? (ot(),
            dt("div", Qv, K[13] || (K[13] = [j("div", null, [j("div", null, [j("div"), j("div"), j("div"), j("div"), j("div"), j("div"), j("div"), j("div"), j("div")])], -1)]))) : ln("", !0), j("div", {
                class: ce(["version", {
                    bar: v.value && n.value == "success"
                }])
            }, Xe(r.value), 3)])])]))
        }
    }
      , tk = "ألعاب PG الرسمية والأصلية"
      , nk = "لم يتم العثور على معرف المعاملة"
      , ik = "يُرجى العودة إلى صفحة سجل اللعبة للتحقق من آخر سجل رهان."
      , sk = "انتهت صلاحية الجلسة"
      , rk = "يُرجى العودة إلى صفحة سجل اللعبة للتحقق من آخر سجل رهان."
      , ok = "معامل غير صحيح"
      , ak = "يُرجى العودة إلى صفحة سجل اللعبة للتحقق من آخر سجل رهان."
      , lk = "خطأ داخلي في الخادم"
      , ck = "يرجى تحديث الصفحة أو العودة إلى صفحة سجل اللعبة للتحقق من أحدث سجل رهان."
      , uk = "تنبيه: تم اكتشاف لعبة مزيفة!"
      , fk = "هذه اللعبة ليست أحد منتجات PG SOFT<sup>®</sup> الرسمية! ننصحك بشدة بالتوقف عن اللعب فورًا للحفاظ على سلامتك."
      , dk = "تذكير بسيط"
      , pk = "الألعاب المزيفة غير معتمدة وقد تشكل مخاطر أمنية وتهديدات مالية. يرجى استخدام الموقع الإلكتروني الرسمي للتحقق {link} لضمان حماية حقوقك."
      , _k = "تم اعتماد منتجات PG SOFT<sup>®</sup> واختبارها بشكل صارم ومزدوج من قِبل الهيئات العالمية BMM وGA، مما يضمن أن خوارزمية المولد العشوائي RNG وتنفيذها ورياضيات اللعبة وقواعدها تفي بأعلى معايير الصناعة من حيث الإنصاف والتكامل."
      , hk = "الخطوة 1:"
      , mk = "حدد معرف المعاملة."
      , gk = "الخطوة 2:"
      , bk = "انقر زر تحقق.ستتم إعادة توجيهك تلقائيًا."
      , yk = "* تأكد من أن عنوان URL يتطابق تمامًا."
      , vk = "ألعاب PG الرسمية والأصلية مع Askmebet"
      , kk = "تم التحقق من قبل Askmebet تايلاند فقط."
      , Ak = "اتصال الشبكة غير مستقر"
      , $k = "يرجى تحديث هذه الصفحة والمحاولة مرة أخرى."
      , Tk = "التحقق غير مكتمل"
      , Ek = "قم بإجراء لفة أخرى وتحقق من معرف المعاملة الأخير."
      , wk = "أو"
      , Sk = "حدد أحد معرفات المعاملات في صفحة سجل اللعبة للتحقق مرة أخرى."
      , Ok = "مرخصة في م.م. \nومالطا"
      , Pk = "معتمدة من \nGA / BMM (UKGC)"
      , xk = {
        official: tk,
        nofound: nk,
        nofound_details: ik,
        expired: sk,
        expired_details: rk,
        wrong: ok,
        wrong_details: ak,
        error: lk,
        error_details: ck,
        fake: uk,
        fake_details: fk,
        gentle: dk,
        reminder: pk,
        certified: _k,
        step1: hk,
        step1_details: mk,
        step2: gk,
        step2_details: bk,
        chars: yk,
        official_th: vk,
        th_only: kk,
        unstable: Ak,
        unstable_details: $k,
        incompleted: Tk,
        incompleted_return: Ek,
        incompleted_or: wk,
        incompleted_history: Sk,
        licensed_top: Ok,
        certified_top: Pk
    }
      , Ik = "Rəsmi və Orijinal PG Oyunları"
      , Ck = "Tranzaksiya İD nömrəsi tapılmadı"
      , Rk = "Sonuncu mərc qeydini yoxlamaq üçün oyun tarixçəsinə qayıdın."
      , Lk = "Seans Müddəti Bitdi"
      , Nk = "Sonuncu mərc qeydini yoxlamaq üçün oyun tarixçəsinə qayıdın."
      , Dk = "Yanlış Parametr"
      , Mk = "Sonuncu mərc qeydini yoxlamaq üçün oyun tarixçəsinə qayıdın."
      , Fk = "Daxili Server Xətası"
      , Gk = "Sonuncu mərc qeydini yoxlamaq üçün oyun tarixçəsinə qayıdın və ya səhifəni yeniləyin."
      , Uk = "DIQQƏT: Saxta Oyun Aşkar Edildi!"
      , jk = "Bu oyun rəsmi PG SOFT<sup>®</sup> məhsulu DEYİL! Təhlükəsizliyiniz üçün oyunu dərhal dayandırmağınızı qətiyyətlə tövsiyə edirik."
      , Bk = "Xatırlatma"
      , zk = "Saxta oyunlar sertifikatlaşdırılmayıb, təhlükəsizlik riski və maliyyə təhlükəsi təşkil edə bilər. Hüquqlarınızın qorunduğuna əmin olmaq üçün rəsmi yoxlanış veb-saytından istifadə edin: {link} "
      , Vk = 'Qlobal təşkilatlar "BMM" və "GA" tərəfindən RNG alqoritmi, onun icrası, oyunun riyaziyyatı,\nədalətlilik və dürüstlük sahəsində ən yüksək sektor standartlarına cavab verən qaydalar\nüzrə ikiqat sertifikatlaşdırma və testdən keçirilib.'
      , Hk = "ADDIM 1:"
      , Kk = "Tranzaksiya İD nömrələrindən birini seçin."
      , qk = "ADDIM 2:"
      , Wk = '"Yoxla" düyməsinə klikləyin. Avtomatik yönləndiriləcəksiniz.'
      , Yk = "* URL-in tam uyğun gəldiyinə əmin olun."
      , Xk = "Askmebet ilə Rəsmi və Orijinal PG Oyunları"
      , Jk = "Yalnız Askmebet Tailand tərəfindən təsdiq olunub."
      , Qk = "Qeyri-stabil şəbəkə bağlantısı"
      , Zk = "Bu səhifəni təzələyin və bir daha cəhd edin."
      , e0 = "Verifikasiya natamamdır"
      , t0 = "Yenidən fırladın və ən son Tranzaksiya İD-i təsdiq edin."
      , n0 = "VƏ YA"
      , i0 = "Yenidən təsdiq etmək üçün oyun tarixçəsi səhifəsində Tranzakisiya İD-lərindən birini seçin."
      , s0 = "Lisenziya:\nBK və Malta"
      , r0 = "Sertifikat: GA\n/ BMM (UKGC)"
      , o0 = {
        official: Ik,
        nofound: Ck,
        nofound_details: Rk,
        expired: Lk,
        expired_details: Nk,
        wrong: Dk,
        wrong_details: Mk,
        error: Fk,
        error_details: Gk,
        fake: Uk,
        fake_details: jk,
        gentle: Bk,
        reminder: zk,
        certified: Vk,
        step1: Hk,
        step1_details: Kk,
        step2: qk,
        step2_details: Wk,
        chars: Yk,
        official_th: Xk,
        th_only: Jk,
        unstable: Qk,
        unstable_details: Zk,
        incompleted: e0,
        incompleted_return: t0,
        incompleted_or: n0,
        incompleted_history: i0,
        licensed_top: s0,
        certified_top: r0
    }
      , a0 = "Официални и оригинални игри от PG"
      , l0 = "Идентификаторът на трансакцията не бе намерен"
      , c0 = "Моля, върнете се на страницата с историята на играта, за да проверите последния запис за залозите."
      , u0 = "Сесията е изтекла"
      , f0 = "Моля, върнете се на страницата с историята на играта, за да проверите последния запис за залозите."
      , d0 = "Неправилен параметър"
      , p0 = "Моля, върнете се на страницата с историята на играта, за да проверите последния запис за залозите."
      , _0 = "Вътрешна грешка на сървъра"
      , h0 = "Моля, опреснете или се върнете на страницата с историята на играта, за да проверите последния запис за залозите."
      , m0 = "ВНИМАНИЕ: Установена е фалшифицирана игра!"
      , g0 = "Тази игра НЕ е официален продукт на PG SOFT<sup>®</sup>! Настоятелно препоръчваме да спрете да играете незабавно с цел Вашата безопасност."
      , b0 = "Любезно напомняне"
      , y0 = "Пиратските игри не са сертифицирани и може да доведат до рискове за сигурността и финансови заплахи. Моля, използвайте официалния уебсайт за проверка {link}, за да бъдат правата Ви защитени."
      , v0 = "Продуктите на PG SOFT<sup>®</sup> са преминали строго двойно сертифициране и са тествани от глобалните органи BMM и GA, за да се гарантира, че алгоритъмът на RNG и внедряването, математиката на играта и правилата изпълняват най-високите изисквания в сектора за добросъвестност и честност."
      , k0 = "СТЪПКА 1:"
      , A0 = "Изберете един от идентификаторите на трансакция."
      , $0 = "СТЪПКА 2:"
      , T0 = "Кликнете върху бутона за проверка. Ще бъдете пренасочени автоматично."
      , E0 = "* Уверете се, че URL адресите съвпадат точно."
      , w0 = "Официални и оригинални игри от PG с Askmebet"
      , S0 = "Потвърдено само от Askmebet Тайланд."
      , O0 = "Нестабилна Връзка с Мрежата"
      , P0 = "Моля, опреснете тази страница и опитайте отново."
      , x0 = "Незавършена Проверка"
      , I0 = "Завъртете повторно и проверете последния Идентификатор на Трансакция."
      , C0 = "ИЛИ"
      , R0 = "Изберете един от Идентификаторите на Трансакция от страницата с история на играта, за да проверите отново."
      , L0 = "Лицензирано в Обединеното\nкралство и Малта"
      , N0 = "Сертифицирано от \nGA / BMM (UKGC)"
      , D0 = {
        official: a0,
        nofound: l0,
        nofound_details: c0,
        expired: u0,
        expired_details: f0,
        wrong: d0,
        wrong_details: p0,
        error: _0,
        error_details: h0,
        fake: m0,
        fake_details: g0,
        gentle: b0,
        reminder: y0,
        certified: v0,
        step1: k0,
        step1_details: A0,
        step2: $0,
        step2_details: T0,
        chars: E0,
        official_th: w0,
        th_only: S0,
        unstable: O0,
        unstable_details: P0,
        incompleted: x0,
        incompleted_return: I0,
        incompleted_or: C0,
        incompleted_history: R0,
        licensed_top: L0,
        certified_top: N0
    }
      , M0 = "অফিসিয়াল এবং প্রকৃত PG গেমস"
      , F0 = "লেনদেনের ID পাওয়া যায়নি"
      , G0 = "বাজির সর্বশেষ রেকর্ড যাচাই করতে গেমের ইতিহাস পেজে ফিরে যান।"
      , U0 = "সেশনের মেয়াদ শেষ হয়ে গেছে"
      , j0 = "বাজির সর্বশেষ রেকর্ড যাচাই করতে গেমের ইতিহাস পেজে ফিরে যান।"
      , B0 = "ভুল প্যারামিটার"
      , z0 = "বাজির সর্বশেষ রেকর্ড যাচাই করতে গেমের ইতিহাস পেজে ফিরে যান।"
      , V0 = "আভ্যন্তরীণ সার্ভারের ত্রুটি"
      , H0 = "বাজির সর্বশেষ রেকর্ড যাচাই করতে রিফ্রেশ করুন অথবা গেমের ইতিহাস পেজে ফিরে যান।"
      , K0 = "সতর্কীকরণ: নকল গেম ধরা পড়েছে!"
      , q0 = "এই গেমটি একটি অফিসিয়াল PG SOFT<sup>®</sup> পণ্য নয়! আমরা আপনার নিরাপত্তার জন্য অবিলম্বে খেলা বন্ধ করার পরামর্শ দিচ্ছি।"
      , W0 = "জেন্টল রিমাইন্ডার"
      , Y0 = "জাল গেমগুলো প্রত্যয়িত নয় এবং এগুলো নিরাপত্তা ঝুঁকি এবং আর্থিক হুমকি সৃষ্টি করতে পারে। আপনার অধিকার সুরক্ষিত আছে তা নিশ্চিত করতে অনুগ্রহ করে অফিসিয়াল ভেরিফিকেশন ওয়েবসাইট {link} ব্যবহার করুন।"
      , X0 = "PG SOFT<sup>®</sup>-এর পণ্যগুলি BMM ও GA-এর মতো কর্তৃপক্ষ দ্বারা কঠোরভাবে দ্বৈত প্রত্যয়িত এবং পরীক্ষিত।এতে RNG অ্যালগরিদম ও তার বাস্তবায়ন, গেমের গণিত, বিধির ন্যায্যতা এবং অখণ্ডতাও রয়েছে।উপরোক্ত বৈশিষ্ট্যগুলি শিল্পের সর্বোচ্চ মান সুনিশ্চিত করে।"
      , J0 = "ধাপ 1:"
      , Q0 = "এর মধ্যে থেকে একটি নির্বাচন করুন লেনদেন সম্পর্কিত ID"
      , Z0 = "ধাপ 2:"
      , eA = "যাচাইকরণ বাটনে ক্লিক করুন। আপনাকে স্বয়ংক্রিয়ভাবে পুনঃনির্দেশিত করা হবে।"
      , tA = "* URL-টি যে অপরিবর্তিত তা নিশ্চিত করুন।"
      , nA = "Askmebet-এ অফিসিয়াল এবং আসল PG গেমস"
      , iA = "শুধুমাত্র Askmebet থাইল্যান্ড দ্বারা যাচাইকৃত।"
      , sA = "অস্থিতিশীল নেটওয়ার্ক সংযোগ"
      , rA = "অনুগ্রহ করে পেজটি রিফ্রেশ করুন এবং পুনরায় চেষ্টা করুন।"
      , oA = "যাচাইকরণ অসম্পূর্ণ"
      , aA = "পুনরায় স্পিন করুন ও সর্বশেষ লেনদেন সম্পর্কিত ID-টি যাচাই করুন।"
      , lA = "অথবা"
      , cA = "পুনরায় যাচাই করতে গেমের ইতিহাস পেজে লেনদেন সম্পর্কিত ID থেকে একটি নির্বাচন করুন"
      , uA = "UK ও মাল্টাতে\nলাইসেন্সকৃত"
      , fA = "GA / BMM(UKGC)\nদ্বারা সার্টিফাইড"
      , dA = {
        official: M0,
        nofound: F0,
        nofound_details: G0,
        expired: U0,
        expired_details: j0,
        wrong: B0,
        wrong_details: z0,
        error: V0,
        error_details: H0,
        fake: K0,
        fake_details: q0,
        gentle: W0,
        reminder: Y0,
        certified: X0,
        step1: J0,
        step1_details: Q0,
        step2: Z0,
        step2_details: eA,
        chars: tA,
        official_th: nA,
        th_only: iA,
        unstable: sA,
        unstable_details: rA,
        incompleted: oA,
        incompleted_return: aA,
        incompleted_or: lA,
        incompleted_history: cA,
        licensed_top: uA,
        certified_top: fA
    }
      , pA = "Oficiální a originální hry PG"
      , _A = "ID transakce nebylo nalezeno"
      , hA = "Vraťte se prosím na stránku historie her a ověřte si poslední záznam o sázce."
      , mA = "Platnost relace skončila"
      , gA = "Vraťte se prosím na stránku historie her a ověřte si poslední záznam o sázce."
      , bA = "Nesprávný parametr"
      , yA = "Vraťte se prosím na stránku historie her a ověřte si poslední záznam o sázce."
      , vA = "Chyba interního serveru"
      , kA = "Pro ověření posledního záznamu o sázce obnovte stránku nebo se vraťte na historii her."
      , AA = "POZOR: Objevena zfalšovaná hra!"
      , $A = "Tato hra NENÍ oficiálním produktem společnosti PG SOFT<sup>®</sup>! V zájmu vaší bezpečnosti důrazně doporučujeme, abyste okamžitě přestali hrát."
      , TA = "Drobná připomínka"
      , EA = "Neautorizované hry nejsou certifikované a mohou představovat bezpečnostní riziko i finanční hrozbu. Pro ochranu svých práv využijte oficiální ověřovací webovou stránku {link}."
      , wA = "Produkty PG SOFT<sup>®</sup> byly důkladně certifikovány a testovány mezinárodními autoritami BMM a GA.Tyto organizace ověřily, že algoritmus RNG, herní matematika a pravidla splňují nejvyšší standardy spravedlnosti a integrity v oboru."
      , SA = "KROK 1:"
      , OA = "Vyber jedno z ID transakce."
      , PA = "KROK 2:"
      , xA = "Klikni na tlačítko ověřit. Přesměrování je automatické."
      , IA = "* Ujistěte se, že adresa URL je správná."
      , CA = "Oficiální a originální hry PG s Askmebet"
      , RA = "Ověřeno pouze Askmebet Thailand."
      , LA = "Nestabilní síťové připojení"
      , NA = "Obnovte stránku a zkuste to znovu."
      , DA = "Ověření nebylo dokončeno"
      , MA = "Znovu roztočte a ověřte nejnovější ID transakce."
      , FA = "NEBO"
      , GA = "Vyberte jedno z ID transakcí na stránce s historií hry a proveďte ověření znovu."
      , UA = "Licencováno ve\nSpojeném království \na na Maltě"
      , jA = "Certifikováno\norganizacemi\nGA / BMM (UKGC)"
      , BA = {
        official: pA,
        nofound: _A,
        nofound_details: hA,
        expired: mA,
        expired_details: gA,
        wrong: bA,
        wrong_details: yA,
        error: vA,
        error_details: kA,
        fake: AA,
        fake_details: $A,
        gentle: TA,
        reminder: EA,
        certified: wA,
        step1: SA,
        step1_details: OA,
        step2: PA,
        step2_details: xA,
        chars: IA,
        official_th: CA,
        th_only: RA,
        unstable: LA,
        unstable_details: NA,
        incompleted: DA,
        incompleted_return: MA,
        incompleted_or: FA,
        incompleted_history: GA,
        licensed_top: UA,
        certified_top: jA
    }
      , zA = "Officielle og ægte PG-spil"
      , VA = "Transaktions-id ikke fundet"
      , HA = "Returnér til spillets historiksiden for at bekræfte den seneste indsats."
      , KA = "Session udløbet"
      , qA = "Returnér til spillets historiksiden for at bekræfte den seneste indsats."
      , WA = "Forkert parameter"
      , YA = "Returnér til spillets historiksiden for at bekræfte den seneste indsats."
      , XA = "Intern serverfejl"
      , JA = "Opdatér eller returnér til spillets historiksiden for at bekræfte den seneste indsats."
      , QA = "OBS: Uægte spil detekteret!"
      , ZA = "Dette spil er IKKE et officielt PG SOFT<sup>®</sup>-produkt! Vi anbefaler på det kraftigste, at du af sikkerhedsmæssige årsager omgående stopper med at spille."
      , e$ = "Lille påmindelse"
      , t$ = "Uægte spil er ikke certificeret, og kan udgøre sikkerhedsrisici og finansielle trusler. Brug den officielle verifikationshjemmeside {link} for at sikre, at dine rettigheder er beskyttet."
      , n$ = "PG SOFT<sup>®</sup>-produkter er dobbeltcertificeret og testet af de globale myndigheder BMM og GA, hvilket sikrer, at RNG-algoritme og implementering, spilmatematik og regler overholder de højeste standarder for fairhed og integritet."
      , i$ = "TRIN 1:"
      , s$ = "Vælg et transaktions-id."
      , r$ = "TRIN 2:"
      , o$ = "Klik på knappen Bekræft. Du videresendes automatisk."
      , a$ = "*Sørg for, at URL-adressen er en nøjagtig match."
      , l$ = "Officielle og ægte PG-spil med Askmebet"
      , c$ = "Kun verificeret af Askmebet Thailand."
      , u$ = "Ustabil netværksforbindelse"
      , f$ = "Opdatér denne side og prøv igen."
      , d$ = "Bekræftelse er ikke fuldført"
      , p$ = "Genspin og bekræft det seneste transaktions-id."
      , _$ = "ELLER"
      , h$ = "Vælg et transaktions-id på siden spilhistorik, som skal bekræftes igen."
      , m$ = "Licenseret i Storbritannien\nog Malta"
      , g$ = "Certificeret af GA /\nBMM (UKGC)"
      , b$ = {
        official: zA,
        nofound: VA,
        nofound_details: HA,
        expired: KA,
        expired_details: qA,
        wrong: WA,
        wrong_details: YA,
        error: XA,
        error_details: JA,
        fake: QA,
        fake_details: ZA,
        gentle: e$,
        reminder: t$,
        certified: n$,
        step1: i$,
        step1_details: s$,
        step2: r$,
        step2_details: o$,
        chars: a$,
        official_th: l$,
        th_only: c$,
        unstable: u$,
        unstable_details: f$,
        incompleted: d$,
        incompleted_return: p$,
        incompleted_or: _$,
        incompleted_history: h$,
        licensed_top: m$,
        certified_top: g$
    }
      , y$ = "Offizielle und echte PG-Spiele"
      , v$ = "Transaktions-ID nicht gefunden"
      , k$ = "Bitte kehren Sie zur Seite des Spielverlaufs zurück, um den letzten Einsatz zu überprüfen."
      , A$ = "Sitzung Abgelaufen"
      , $$ = "Bitte kehren Sie zur Seite des Spielverlaufs zurück, um den letzten Einsatz zu überprüfen."
      , T$ = "Falscher Parameter"
      , E$ = "Bitte kehren Sie zur Seite des Spielverlaufs zurück, um den letzten Einsatz zu überprüfen."
      , w$ = "Interner Serverfehler"
      , S$ = "Bitte aktualisieren Sie die Seite oder kehren Sie zum Spielverlauf zurück, um den letzten Einsatz zu überprüfen."
      , O$ = "ACHTUNG: Manipuliertes Spiel entdeckt!"
      , P$ = "Dieses Spiel ist KEIN offizielles PG SOFT<sup>®</sup>-Produkt! Wir raten Ihnen dringend, das Spiel zu Ihrer eigenen Sicherheit sofort zu beenden."
      , x$ = "Freundliche Erinnerung"
      , I$ = "Gefälschte Spiele sind nicht zertifiziert und können ein Sicherheitsrisiko und eine finanzielle Bedrohung darstellen. Bitte verwenden Sie die offizielle Verifizierungs-Website {link}, um sicherzustellen, dass Ihre Rechte geschützt sind."
      , C$ = "PG SOFT<sup>®</sup>-Produkte wurden von den Behörden BMM und GA doppelt zertifiziert und getestet, damit der RNG-Algorithmus, die Implementierung, die Spielmathematik und Regeln den höchsten Branchenstandards für Fairness und Integrität entsprechen."
      , R$ = "1. SCHRITT:"
      , L$ = "Wählen Sie eine der Transaktions-IDs."
      , N$ = "2. SCHRITT:"
      , D$ = "Klicken Sie auf die Schaltfläche „Verifizieren“. Sie werden dann automatisch weitergeleitet."
      , M$ = "* Stellen Sie sicher, dass die URL genau übereinstimmt."
      , F$ = "Offizielle und echte PG-Spiele mit Askmebet"
      , G$ = "Nur von AskBet Thailand verifiziert."
      , U$ = "Instabile Netzwerkverbindung"
      , j$ = "Bitte aktualisieren Sie diese Seite und versuchen Sie es erneut."
      , B$ = "Verifizierung unvollständig"
      , z$ = "Drehen Sie erneut und verifizieren Sie die neueste Transaktions-ID."
      , V$ = "ODER"
      , H$ = "Wählen Sie eine der Transaktions-IDs auf der Seite „Spielverlauf“ aus, um sie erneut zu überprüfen."
      , K$ = "Im Vereinigten Königreich und in\nMalta lizenziert"
      , q$ = "Durch GA /\nBMM zertifiziert (UKGC)"
      , W$ = {
        official: y$,
        nofound: v$,
        nofound_details: k$,
        expired: A$,
        expired_details: $$,
        wrong: T$,
        wrong_details: E$,
        error: w$,
        error_details: S$,
        fake: O$,
        fake_details: P$,
        gentle: x$,
        reminder: I$,
        certified: C$,
        step1: R$,
        step1_details: L$,
        step2: N$,
        step2_details: D$,
        chars: M$,
        official_th: F$,
        th_only: G$,
        unstable: U$,
        unstable_details: j$,
        incompleted: B$,
        incompleted_return: z$,
        incompleted_or: V$,
        incompleted_history: H$,
        licensed_top: K$,
        certified_top: q$
    }
      , Y$ = "Επίσημα και Γνήσια Παιχνίδια της PG"
      , X$ = "Το ID Συναλλαγής δεν βρέθηκε"
      , J$ = "Επιστρέψτε στη σελίδα ιστορικού παιχνιδιών για να επαληθεύσετε το τελευταίο αρχείο στοιχημάτων."
      , Q$ = "Η Συνεδρία Έληξε"
      , Z$ = "Επιστρέψτε στη σελίδα ιστορικού παιχνιδιών για να επαληθεύσετε το τελευταίο αρχείο στοιχημάτων."
      , eT = "Εσφαλμένη Παράμετρος"
      , tT = "Επιστρέψτε στη σελίδα ιστορικού παιχνιδιών για να επαληθεύσετε το τελευταίο αρχείο στοιχημάτων."
      , nT = "Εσωτερικό Σφάλμα Διακομιστή"
      , iT = "Ανανεώστε ή επιστρέψτε στη σελίδα ιστορικού παιχνιδιών για να επαληθεύσετε το τελευταίο αρχείο στοιχημάτων."
      , sT = "ΠΡΟΣΟΧΗ: Εντοπίστηκε Πλαστό Παιχνίδι!"
      , rT = "Αυτό το παιχνίδι ΔΕΝ είναι επίσημο προϊόν της PG SOFT<sup>®</sup>! Για την ασφάλειά σας, σας συνιστούμε να σταματήσετε αμέσως το παιχνίδι."
      , oT = "Ευγενική Υπενθύμιση"
      , aT = "Τα πλαστά παιχνίδια δεν είναι πιστοποιημένα και ενδέχεται να εγκυμονούν κινδύνους για την ασφάλεια ή να αποτελούν απειλή για τα οικονομικά των παικτών. Χρησιμοποιήστε τον επίσημο ιστότοπο επαλήθευσης {link} για να διασφαλίσετε την προστασία των δικαιωμάτων σας."
      , lT = "Τα προϊόντα PG SOFT<sup>®</sup> έχουν υποβληθεί σε αυστηρή διπλή πιστοποίηση και δοκιμή από τις παγκόσμιες αρχές BMM και GA, διασφαλίζοντας ότι ο αλγόριθμος και η εφαρμογή RNG, τα μαθηματικά και οι κανόνες παιχνιδιού πληρούν τις υψηλότερες προδιαγραφές δικαιοσύνης και ακεραιότητας."
      , cT = "ΒΗΜΑ 1:"
      , uT = "Επιλέξτε ένα ID Συναλλαγής."
      , fT = "ΒΗΜΑ 2:"
      , dT = "Πατήστε το κουμπί Επαλήθευση. Θα μεταφερθείτε αυτόματα."
      , pT = "* Βεβαιωθείτε ότι η διεύθυνση URL ταιριάζει ακριβώς."
      , _T = "Επίσημα και Γνήσια Παιχνίδια της PG με την Askmebet"
      , hT = "Επαληθευμένο μόνο από την Askmebet Thailand."
      , mT = "Ασταθής Σύνδεση Δικτύου"
      , gT = "Ανανεώστε αυτή τη σελίδα και δοκιμάστε ξανά."
      , bT = "Μη Ολοκληρωμένη Επαλήθευση"
      , yT = "Περιστρέψτε εκ νέου και επαληθεύσετε το τελευταίο ID Συναλλαγής."
      , vT = "Ή"
      , kT = "Επιλέξτε ένα από τα ID Συναλλαγής στη σελίδα του ιστορικού παιχνιδιού για να επαληθεύσετε ξανά."
      , AT = "Αδειοδοτημένη στο\nΗνωμένο Βασίλειο\nκαι στη Μάλτα"
      , $T = "Πιστοποιημένη\nαπό GA / BMM\n(UKGC)"
      , TT = {
        official: Y$,
        nofound: X$,
        nofound_details: J$,
        expired: Q$,
        expired_details: Z$,
        wrong: eT,
        wrong_details: tT,
        error: nT,
        error_details: iT,
        fake: sT,
        fake_details: rT,
        gentle: oT,
        reminder: aT,
        certified: lT,
        step1: cT,
        step1_details: uT,
        step2: fT,
        step2_details: dT,
        chars: pT,
        official_th: _T,
        th_only: hT,
        unstable: mT,
        unstable_details: gT,
        incompleted: bT,
        incompleted_return: yT,
        incompleted_or: vT,
        incompleted_history: kT,
        licensed_top: AT,
        certified_top: $T
    }
      , ET = "Official and Genuine PG Games"
      , wT = "Transaction ID not found"
      , ST = "Please return to the game history page to verify the latest bet record."
      , OT = "Session Expired"
      , PT = "Please return to the game history page to verify the latest bet record."
      , xT = "Incorrect Parameter"
      , IT = "Please return to the game history page to verify the latest bet record."
      , CT = "Internal Server Error"
      , RT = "Please refresh or return to the game history page to verify the latest bet record."
      , LT = "CAUTION: Counterfeit Game Detected!"
      , NT = "This game is NOT an official PG SOFT<sup>®</sup> product! We strongly advise you to stop playing immediately for your safety."
      , DT = "Gentle Reminder"
      , MT = "Counterfeit games are not certified and may pose security risks and financial threats. Please use the official verification website {link} to ensure your rights are protected."
      , FT = "PG SOFT<sup>®</sup> products have been rigorously dual certified and tested by global authorities BMM and GA, ensuring the RNG algorithm and implementation, game mathematics, and rules meet the highest industry standards for fairness and integrity."
      , GT = "STEP 1:"
      , UT = "Select one of the Transaction ID."
      , jT = "STEP 2:"
      , BT = "Click on the Verify button. You'll be auto-redirected."
      , zT = "* Ensure the URL matches exactly."
      , VT = "Official and Genuine PG Games with Askmebet"
      , HT = "Verified by Askmebet Thailand only."
      , KT = "Unstable Network Connection"
      , qT = "Please refresh this page and try again."
      , WT = "Verification Incomplete"
      , YT = "Respin and verify the latest Transaction ID."
      , XT = "OR"
      , JT = "Select one of the Transaction ID in the game history page to verify again."
      , QT = "Licensed in UK and Malta"
      , ZT = "Certified by GA / BMM (UKGC)"
      , e1 = {
        official: ET,
        nofound: wT,
        nofound_details: ST,
        expired: OT,
        expired_details: PT,
        wrong: xT,
        wrong_details: IT,
        error: CT,
        error_details: RT,
        fake: LT,
        fake_details: NT,
        gentle: DT,
        reminder: MT,
        certified: FT,
        step1: GT,
        step1_details: UT,
        step2: jT,
        step2_details: BT,
        chars: zT,
        official_th: VT,
        th_only: HT,
        unstable: KT,
        unstable_details: qT,
        incompleted: WT,
        incompleted_return: YT,
        incompleted_or: XT,
        incompleted_history: JT,
        licensed_top: QT,
        certified_top: ZT
    }
      , t1 = "Juegos oficiales y originales de PG"
      , n1 = "ID de transacción no encontrado"
      , i1 = "Vuelva a la página del historial de partidas para verificar el último registro de apuestas."
      , s1 = "La sesión ha caducado"
      , r1 = "Vuelva a la página del historial de partidas para verificar el último registro de apuestas."
      , o1 = "Parámetro incorrecto"
      , a1 = "Vuelva a la página del historial de partidas para verificar el último registro de apuestas."
      , l1 = "Error Interno del Servidor"
      , c1 = "Actualice o vuelva a la página del historial de partidas para verificar el último registro de apuestas."
      , u1 = "ATENCIÓN: ¡Se ha detectado un juego falsificado!"
      , f1 = "¡Este juego NO es un producto oficial de PG SOFT<sup>®</sup>! Por su seguridad, le recomendamos encarecidamente que deje de jugar de inmediato."
      , d1 = "Recordatorio"
      , p1 = "Los juegos falsificados no están certificados y pueden plantear riesgos de seguridad y financieros. Utilice el sitio web oficial de verificación {link} para garantizar la protección de sus derechos."
      , _1 = "Los productos PG SOFT<sup>®</sup> han sido certificados y probados por BMM y GA, garantizando que el algoritmo RNG y su implementación, las matemáticas del juego y las reglas cumplen con los más altos estándares de imparcialidad e integridad."
      , h1 = "PASO 1:"
      , m1 = "Seleccione un ID de transacción."
      , g1 = "PASO 2:"
      , b1 = "Haga clic en el botón Verificar. Se le redirigirá automáticamente."
      , y1 = "* Compruebe que la URL coincide exactamente."
      , v1 = "Juegos oficiales y originales de PG con Askmebet"
      , k1 = "Verificado solo por Askmebet Tailandia."
      , A1 = "Conexión de Red Inestable"
      , $1 = "Actualice la página y vuelva a intentarlo."
      , T1 = "Verificación Incompleta"
      , E1 = "Regire y verifique el último ID de Transacción."
      , w1 = "O"
      , S1 = "Seleccione un ID Transacción en la página del historial de juego para volver a verificarlo."
      , O1 = "Con licencias de\nReino Unido y Malta"
      , P1 = "Con certificación de GA /\nBMM (UKGC)"
      , x1 = {
        official: t1,
        nofound: n1,
        nofound_details: i1,
        expired: s1,
        expired_details: r1,
        wrong: o1,
        wrong_details: a1,
        error: l1,
        error_details: c1,
        fake: u1,
        fake_details: f1,
        gentle: d1,
        reminder: p1,
        certified: _1,
        step1: h1,
        step1_details: m1,
        step2: g1,
        step2_details: b1,
        chars: y1,
        official_th: v1,
        th_only: k1,
        unstable: A1,
        unstable_details: $1,
        incompleted: T1,
        incompleted_return: E1,
        incompleted_or: w1,
        incompleted_history: S1,
        licensed_top: O1,
        certified_top: P1
    }
      , I1 = "Ametlikud ja Ehtsad PG Mängud"
      , C1 = "Ülekande ID-d ei leitud"
      , R1 = "Mine tagasi mängu ajaloo lehele, et verifitseerida viimaseid panuseid."
      , L1 = "Sessioon aegus"
      , N1 = "Mine tagasi mängu ajaloo lehele, et verifitseerida viimaseid panuseid."
      , D1 = "Vale parameeter"
      , M1 = "Mine tagasi mängu ajaloo lehele, et verifitseerida viimaseid panuseid."
      , F1 = "Serveri sisemine viga"
      , G1 = "Värskenda või mine tagasi mängu ajaloo lehele, et verifitseerida viimaseid panuseid."
      , U1 = "HOIATUS! Tuvastati Võltsitud Mäng!"
      , j1 = "See mäng EI OLE ametlik PG SOFT<sup>®</sup>-i toode! Teie turvalisuse huvides soovitame tungivalt kohe mängimise lõpetada."
      , B1 = "Tagasihoidlik meeldetuletus"
      , z1 = "Võltsitud mängud pole verifitseeritud ja nendega võivad kaasneda turva- ja finantsriskid. Palun kasuta oma õiguste kaitses veendumiseks ametlikku verifitseerimissaiti {link}."
      , V1 = "PG SOFT<sup>®</sup>-i tooteid on rangelt kahekordselt sertifitseerinud ja testinud ülemaailmsed asutused BMM ja GA, tagades, et RNG algoritm ja selle rakendamine, mängu matemaatika ja reeglid vastavad valdkonna kõrgeimatele õigluse ja terviklikkuse standarditele."
      , H1 = "1. SAMM:"
      , K1 = "Vali üks Ülekande ID"
      , q1 = "2. SAMM:"
      , W1 = "Vajuta nupule OK. Sind suunatakse ümber."
      , Y1 = "* Kontrolli, et URL oleks sama."
      , X1 = "Ametlikud ja Ehtsad PG Mängud koos Askmebetiga"
      , J1 = "Verifitseerinud ainult Askmebet Thailand."
      , Q1 = "Ebastabiilne internetiühendus"
      , Z1 = "Palun värskenda lehekülge ja proovi uuesti."
      , eE = "Verifitseerimine mittetäielik"
      , tE = "Tee kordusspinn ja verifitseeri viimane Ülekande ID."
      , nE = "VÕI"
      , iE = "Vali mängu ajaloo lehel üks Ülekande ID, et verifitseerida veelkord"
      , sE = "Litsentseeri-\ntud ÜK-s ja Maltal\n"
      , rE = "Sertifitseeri-\ntud GA / BMM (UKGC) poolt"
      , oE = {
        official: I1,
        nofound: C1,
        nofound_details: R1,
        expired: L1,
        expired_details: N1,
        wrong: D1,
        wrong_details: M1,
        error: F1,
        error_details: G1,
        fake: U1,
        fake_details: j1,
        gentle: B1,
        reminder: z1,
        certified: V1,
        step1: H1,
        step1_details: K1,
        step2: q1,
        step2_details: W1,
        chars: Y1,
        official_th: X1,
        th_only: J1,
        unstable: Q1,
        unstable_details: Z1,
        incompleted: eE,
        incompleted_return: tE,
        incompleted_or: nE,
        incompleted_history: iE,
        licensed_top: sE,
        certified_top: rE
    }
      , aE = "بازی های رسمی و اصلی PG"
      , lE = "شناسه تراکنش پیدا نشد"
      , cE = "لطفاً به صفحه تاریخچه بازی بازگردید تا آخرین سابقه شرط را تایید کنید."
      , uE = "جلسه منقضی شد"
      , fE = "لطفاً به صفحه تاریخچه بازی بازگردید تا آخرین سابقه شرط را تایید کنید."
      , dE = "پارامتر نادرست"
      , pE = "لطفاً به صفحه تاریخچه بازی بازگردید تا آخرین سابقه شرط را تایید کنید."
      , _E = "خطای داخلی سرور"
      , hE = "لطفاً به صفحه تاریخچه بازی بازگردید تا آخرین سابقه شرط را تایید کنید."
      , mE = "احتیاط: بازی تقلبی شناسایی شد!"
      , gE = "این بازی یک محصول رسمی از PG SOFT<sup>®</sup> نیست! ما به شدت توصیه می‌کنیم برای ایمنی خود فوراً بازی را متوقف کنید."
      , bE = "یادآوری دوستانه"
      , yE = "بازی‌های تقلبی تایید نشده‌اند و ممکن است خطرات امنیتی و تهدیدات مالی داشته باشند. لطفاً از وب‌سایت اعتبارسنجی رسمی {link} استفاده کنید تا از حقوق خود محافظت کنید."
      , vE = "محصولات PG SOFT دارای گواهینامه دوگانه اند و توسط مقامات جهانی BMM و GA تست شده اند، تا از الگوریتم RNG و پیاده سازی آن، ریاضیات بازی اطمینان یابید، و قوانین مطابق با بالاترین استانداردهای صنعت برای انصاف و درستی بازی هست."
      , kE = "مرحله 1:"
      , AE = "یکی از شناسه های تراکنش را انتخاب کنید"
      , $E = "مرحله 2:"
      , TE = "بر روی دکمه تایید کلیک کنید شما به طور خودکار هدایت می شوید."
      , EE = "* مطمئن شوید که URL دقیقاً مطابق باشد."
      , wE = "بازی های رسمی و اصلی PG با Askmebet"
      , SE = "تنها توسط Askmebet تایلند تأیید شده است."
      , OE = "اتصال شبکه ناپایدار"
      , PE = "لطفاً این صفحه را بازنشانی کرده و دوباره تلاش کنید."
      , xE = "تأیید ناموفق"
      , IE = "دوباره بچرخانید و آخرین شناسه تراکنش را تأیید کنید."
      , CE = "یا"
      , RE = "یکی از شناسه‌های تراکنش در صفحه تاریخچه بازی را انتخاب کنید تا دوباره تأیید شود."
      , LE = "دارای مجوز در\nانگلستان و مالت"
      , NE = "تأیید شده توسط\nGA / BMM (UKGC)"
      , DE = {
        official: aE,
        nofound: lE,
        nofound_details: cE,
        expired: uE,
        expired_details: fE,
        wrong: dE,
        wrong_details: pE,
        error: _E,
        error_details: hE,
        fake: mE,
        fake_details: gE,
        gentle: bE,
        reminder: yE,
        certified: vE,
        step1: kE,
        step1_details: AE,
        step2: $E,
        step2_details: TE,
        chars: EE,
        official_th: wE,
        th_only: SE,
        unstable: OE,
        unstable_details: PE,
        incompleted: xE,
        incompleted_return: IE,
        incompleted_or: CE,
        incompleted_history: RE,
        licensed_top: LE,
        certified_top: NE
    }
      , ME = "Viralliset ja aidot PG Games -pelit"
      , FE = "Transaktiotunnusta ei löydy"
      , GE = "Palaa pelihistoriasivulle vahvistaaksesi viimeisimmän panostallenteen."
      , UE = "Istunto vanhentunut"
      , jE = "Palaa pelihistoriasivulle vahvistaaksesi viimeisimmän panostallenteen."
      , BE = "Virheellinen parametri"
      , zE = "Palaa pelihistoriasivulle vahvistaaksesi viimeisimmän panostallenteen."
      , VE = "Sisäinen palvelinvirhe"
      , HE = "Päivitä tai palaa pelihistoriasivulle vahvistaaksesi viimeisimmän panostallenteen."
      , KE = "VAROITUS: Väärennetty peli havaittu!"
      , qE = "Tämä peli EI ole virallinen PG SOFT<sup>®</sup> -tuote! Suosittelemme turvallisuutesi vuoksi voimakkaasti, että lopetat pelaamisen välittömästi."
      , WE = "Ystävällinen muistutus"
      , YE = "Väärennettyjä pelejä ei ole varmennettu, ja ne voivat muodostaa tietoturvauhkia ja taloudellisia riskejä. Käytä virallista vahvistussivustoa {link} varmistaaksesi, että oikeutesi on turvattu."
      , XE = "Globaalit viranomaiset BMM ja GA ovat huolellisesti kaksoissertifioineet ja testanneet PG SOFT<sup>®</sup> -tuotteet varmistaen, että niiden RNG-algoritmi ja sen käyttöönotto sekä pelin matematiikka ja säännöt vastaavat alan reiluusstandardeja."
      , JE = "VAIHE 1:"
      , QE = "Valitse yksi transaktiotunnuksista."
      , ZE = "VAIHE 2:"
      , ew = "Napsauta Vahvista-painiketta. Sinut uudelleenohjataan automaattisesti."
      , tw = "* Varmista, että URL on täsmälleen oikein."
      , nw = "Viralliset ja aidot PG Games -pelit Askmebetillä"
      , iw = "Vain Askmebet Thaimaan vahvistama"
      , sw = "Epävakaa Verkkoyhteys"
      , rw = "Päivitä sivu ja yritä uudelleen."
      , ow = "Vahvistusta ei suoritettu"
      , aw = "Pyöräytä Vapaapyöräytys ja vahvista uusin Transaktiotunnus."
      , lw = "TAI"
      , cw = "Valitse pelihistoriasivulta yksi Transaktiotunnuksista vahvistettavaksi uudelleen:"
      , uw = "Lisensoitu Yhdistyneessä\nkuningaskunnassa ja Maltalla"
      , fw = "GA:n /\nBMM:n sertifioima (UKGC)"
      , dw = {
        official: ME,
        nofound: FE,
        nofound_details: GE,
        expired: UE,
        expired_details: jE,
        wrong: BE,
        wrong_details: zE,
        error: VE,
        error_details: HE,
        fake: KE,
        fake_details: qE,
        gentle: WE,
        reminder: YE,
        certified: XE,
        step1: JE,
        step1_details: QE,
        step2: ZE,
        step2_details: ew,
        chars: tw,
        official_th: nw,
        th_only: iw,
        unstable: sw,
        unstable_details: rw,
        incompleted: ow,
        incompleted_return: aw,
        incompleted_or: lw,
        incompleted_history: cw,
        licensed_top: uw,
        certified_top: fw
    }
      , pw = "Jeux officiels et authentiques de PG"
      , _w = "L’ID de transaction est introuvable"
      , hw = "Veuillez retourner à la page de l’historique du jeu pour procéder à la vérification des derniers paris placés."
      , mw = "La session a expiré"
      , gw = "Veuillez retourner à la page de l’historique du jeu pour procéder à la vérification des derniers paris placés."
      , bw = "Paramètre incorrect"
      , yw = "Veuillez retourner à la page de l’historique du jeu pour procéder à la vérification des derniers paris placés."
      , vw = "Erreur de serveur interne"
      , kw = "Veuillez rafraîchir ou retourner à la page de l’historique du jeu pour procéder à la vérification des derniers paris placés."
      , Aw = "ATTENTION : jeu contrefait détecté !"
      , $w = "Ce jeu N’EST PAS un produit officiel de la marque PG SOFT<sup>®</sup> ! Pour votre sécurité, nous vous conseillons vivement d’arrêter immédiatement de jouer."
      , Tw = "Rappel"
      , Ew = "Les jeux de contrefaçon ne sont pas certifiés et peuvent ainsi poser des risques financiers et de sécurité. Consultez le site officiel de vérification prévu à cet effet ({link}) pour vous assurer que vos droits sont protégés."
      , ww = "Les produits PG SOFT<sup>®</sup> ont été rigoureusement et doublement testés et certifiés par BMM et GA, garantissant l’équité et l’impartialité des statistiques de jeu, de ses règles, et de la génération de nombres aléatoires."
      , Sw = "ÉTAPE 1 :"
      , Ow = "Sélectionnez l’une des ID de transaction."
      , Pw = "ÉTAPE 2 :"
      , xw = "Cliquez sur le bouton Vérifier. Vous serez redirigé(e) automatiquement."
      , Iw = "* Assurez-vous que l’adresse URL soit exactement la même."
      , Cw = "Jeux officiels et authentiques de PG en collaboration avec Askmebet"
      , Rw = "Exclusivement vérifié par Askmebet Thaïlande."
      , Lw = "Connexion réseau instable"
      , Nw = "Veuillez rafraîchir la page et réessayer."
      , Dw = "Vérification incomplète"
      , Mw = "Veuillez relancer pour vérifier la dernière ID de transaction."
      , Fw = "OU"
      , Gw = "Sélectionnez l’une des ID de transaction depuis la page de l’historique du jeu pour en vérifier l’authenticité à nouveau."
      , Uw = "Homologué au\nRoyaume-Uni et à Malte"
      , jw = "Certifié par GA /\nBMM (UKGC)"
      , Bw = {
        official: pw,
        nofound: _w,
        nofound_details: hw,
        expired: mw,
        expired_details: gw,
        wrong: bw,
        wrong_details: yw,
        error: vw,
        error_details: kw,
        fake: Aw,
        fake_details: $w,
        gentle: Tw,
        reminder: Ew,
        certified: ww,
        step1: Sw,
        step1_details: Ow,
        step2: Pw,
        step2_details: xw,
        chars: Iw,
        official_th: Cw,
        th_only: Rw,
        unstable: Lw,
        unstable_details: Nw,
        incompleted: Dw,
        incompleted_return: Mw,
        incompleted_or: Fw,
        incompleted_history: Gw,
        licensed_top: Uw,
        certified_top: jw
    }
      , zw = "आधिकारिक और असली PG गेम्स"
      , Vw = "लेनदेन ID नहीं मिली"
      , Hw = "नवीनतम बेट रिकॉर्ड सत्यापन करने के लिए गेम हिस्ट्री पर लौटें।"
      , Kw = "सेशन समाप्त हुआ"
      , qw = "नवीनतम बेट रिकॉर्ड सत्यापन करने के लिए गेम हिस्ट्री पर लौटें।"
      , Ww = "अमान्य पैरामीटर"
      , Yw = "नवीनतम बेट रिकॉर्ड सत्यापन करने के लिए गेम हिस्ट्री पर लौटें।"
      , Xw = "आंतरिक सर्वर त्रुटि"
      , Jw = "नवीनतम बेट रिकॉर्ड सत्यापन करने के लिए गेम हिस्ट्री को रिफ्रेश करें या पेज़ पर वापस लौटें।"
      , Qw = "चेतावनी: नकली गेम पाया गया!"
      , Zw = "यह गेम PG SOFT<sup>®</sup> प्रॉडक्ट का आधिकारिक सेवक हूँ हम आपकी सुरक्षा के लिए आप इस गेम को खेलना बंद करें."
      , eS = "विनम्र अनुस्मारक"
      , tS = "नकली गेम प्रमाणित नहीं होते और इनसे सुरक्षा जोखिम और वित्तीय खतरा उत्पन्न हो सकता है। कृपया अपने अधिकारों की सुरक्षा सुनिश्चित करने के लिए आधिकारिक सत्यापन वेबसाइट {link} का उपयोग करें।"
      , nS = "PG SOFT<sup>®</sup> प्रॉडक्ट्स का वैश्विक प्राधिकरणों BMM और GA द्वारा सख्त तौर पर दोहरा प्रमाणन \nऔर परीक्षण किया गया है, जिससे यह सुनिश्चित होता है कि RNG एल्गोरिदम और \nकार्यान्वयन, गेम गणित और नियम निष्पक्षता और अखंडता \nके लिए उच्चतम इंडस्ट्री मानकों को पूरा करते हैं।"
      , iS = "स्टेप 1:"
      , sS = "कोई एक चुनें लेनदेन ID"
      , rS = "स्टेप 2:"
      , oS = "क्लिक करें सत्यापन बटन पर। आपको स्वत:पुनर्निर्देशित किया जाएगा।"
      , aS = "* सुनिश्चित करें कि URL पूरी तरह से समान है।"
      , lS = "Askmebet के साथ आधिकारिक और असली PG गेम्स"
      , cS = "केवल Askmebet थायलैंड द्वारा सत्यापित।"
      , uS = "अस्थिर नेटवर्क कनेक्शन"
      , fS = "कृपया इस पेज़ को रिफ्रेश कर पुनः प्रयास करें।"
      , dS = "सत्यापन अपूर्ण"
      , pS = "फिर से स्पिन करें और नवीनतम लेनदेन ID सत्यापित करें।"
      , _S = "या"
      , hS = "गेम हिस्ट्री पर कोई एक लेनदेन ID चुनकर पुनः सत्यापित करें :"
      , mS = "यूके और माल्टा\nमें लाइसेंस\nप्राप्त"
      , gS = "GA / BMM (UKGC)\nद्वारा प्रमाणित"
      , bS = {
        official: zw,
        nofound: Vw,
        nofound_details: Hw,
        expired: Kw,
        expired_details: qw,
        wrong: Ww,
        wrong_details: Yw,
        error: Xw,
        error_details: Jw,
        fake: Qw,
        fake_details: Zw,
        gentle: eS,
        reminder: tS,
        certified: nS,
        step1: iS,
        step1_details: sS,
        step2: rS,
        step2_details: oS,
        chars: aS,
        official_th: lS,
        th_only: cS,
        unstable: uS,
        unstable_details: fS,
        incompleted: dS,
        incompleted_return: pS,
        incompleted_or: _S,
        incompleted_history: hS,
        licensed_top: mS,
        certified_top: gS
    }
      , yS = "Hivatalos és eredeti PG-játékok"
      , vS = "A tranzakcióazonosító nem található"
      , kS = "Térjen vissza a játékelőzmények oldalra a legutóbbi tétrekord ellenőrzéséhez."
      , AS = "A munkamenet lejárt"
      , $S = "Térjen vissza a játékelőzmények oldalra a legutóbbi tétrekord ellenőrzéséhez."
      , TS = "Helytelen paraméter"
      , ES = "Térjen vissza a játékelőzmények oldalra a legutóbbi tétrekord ellenőrzéséhez."
      , wS = "Belső kiszolgálóhiba"
      , SS = "Frissítsen vagy térjen vissza a játékelőzmények oldalra a legutóbbi tétrekord ellenőrzéséhez."
      , OS = "FIGYELEM: Hamisított játékot észleltünk!"
      , PS = "Ez a játék NEM hivatalos PG SOFT<sup>®</sup>-termék! Nyomatékosan javasoljuk, hogy a biztonsága érdekében azonnal fejezze be a játékot."
      , xS = "Emlékeztető"
      , IS = "A hamisított játékok nem rendelkeznek tanúsítvánnyal, és biztonsági kockázatot és pénzügyi fenyegetést jelenthetnek. Használja a hivatalos ellenőrző webhelyet {link}, hogy garantálja a jogai védelmét."
      , CS = "A PG SOFT<sup>®</sup>-termékek szigorú kettős tanúsítást kaptak és tesztelésen estek át\na BMM és GA által, így az RNG-algoritmus és -megvalósítás, a játékmatematika és a szabályok megfelelnek\na legszigorúbb iparági szabványoknak a méltányosság és integritás terén."
      , RS = "1. LÉPÉS:"
      , LS = "Válasszon egy tranzakcióazonosítót."
      , NS = "2. LÉPÉS:"
      , DS = "Kattintson az Ellenőrzés gombra. Automatikusan átirányítják."
      , MS = "* Gondoskodjon arról, hogy az URL-cím pontosan megegyezzen."
      , FS = "Hivatalos és eredeti PG-játékok az Askmebettel"
      , GS = "Csak az Askmebet Thailand ellenőrizte."
      , US = "Instabil hálózati kapcsolat"
      , jS = "Frissítse ezt az oldalt, és próbálja újra."
      , BS = "Hiányos ellenőrzés"
      , zS = "Pörgessen újra, és ellenőrizze a legutóbbi tranzakcióazonosítót."
      , VS = "VAGY"
      , HS = "Az ismételt ellenőrzéshez válassza ki a tranzakcióazonosítók egyikét a játékelőzmények oldalon."
      , KS = "Engedély Egyesült\nKirályság/Málta"
      , qS = "GA / BMM (UKGC)\náltal tanúsítva"
      , WS = {
        official: yS,
        nofound: vS,
        nofound_details: kS,
        expired: AS,
        expired_details: $S,
        wrong: TS,
        wrong_details: ES,
        error: wS,
        error_details: SS,
        fake: OS,
        fake_details: PS,
        gentle: xS,
        reminder: IS,
        certified: CS,
        step1: RS,
        step1_details: LS,
        step2: NS,
        step2_details: DS,
        chars: MS,
        official_th: FS,
        th_only: GS,
        unstable: US,
        unstable_details: jS,
        incompleted: BS,
        incompleted_return: zS,
        incompleted_or: VS,
        incompleted_history: HS,
        licensed_top: KS,
        certified_top: qS
    }
      , YS = "PG-ի պաշտոնական և իրական խաղեր"
      , XS = "Գործարքի ID-ն չի հայտնաբերվել"
      , JS = "Վերադարձեք խաղերի պատմության էջ, որպեսզի հաստատեք ամենավերջին խաղադրույքի գրառումը։"
      , QS = "Խաղաշրջանի ժամանակը սպառվեց"
      , ZS = "Վերադարձեք խաղերի պատմության էջ, որպեսզի հաստատեք ամենավերջին խաղադրույքի գրառումը։"
      , eO = "Սխալ պարամետր"
      , tO = "Վերադարձեք խաղերի պատմության էջ, որպեսզի հաստատեք ամենավերջին խաղադրույքի գրառումը։"
      , nO = "Սերվերի ներքին սխալ"
      , iO = "Թարմացրեք կամ վերադարձեք խաղերի պատմության էջ, որպեսզի հաստատեք ամենավերջին խաղադրույքի գրառումը։"
      , sO = "ՈՒՇԱԴՐՈՒԹՅՈՒՆ․ Հայտնաբերվել է կեղծ խաղ։"
      , rO = "Այս խաղը ՉԻ հանդիսանում PG SOFT<sup>®</sup>-ի պաշտոնական պրոդուկտ։ Մենք իսկապես խորհուրդ ենք տալիս անմիջապես դադարեցնել խաղալը՝ Ձեր իսկ անվտանգությունից ելնելով։"
      , oO = "Փոքրիկ հիշեցում"
      , aO = "Կեղծ խաղերը վկայագրված չեն և կարող են անվտանգության ռիսկեր ու ֆինանսական սպառնալիքներ առաջացնել։ Օգտագործեք հաստատման պաշտոնական կայքէջը ({link}), որպեսզի համոզվեք, որ ձեր իրավունքները պաշտպանված են։"
      , lO = "PG SOFT<sup>®</sup> պրոդուկտները կրկնակի վկայագրվել և փորձարկվել են BMM և GA գլոբալ կառույցների կողմից՝ ապահովելով RNG-ի ալգորիթմը, իրականացումը, խաղի մաթեմատիկան, իսկ կանոնները բավարարում են ոլորտի արդարության և ազնվության ամենաբարձր ստանդարտները։"
      , cO = "ՔԱՅԼ 1․"
      , uO = "Ընտրե՛ք գործարքի ID-ներից մեկը։"
      , fO = "ՔԱՅԼ 2․"
      , dO = "Սեղմե՛ք Հաստատել կոճակը։ Դուք ինքնաբերաբար կվերաուղղորդվեք։"
      , pO = "* Ապահովեք URL-ի ճշգրիտ համընկնումները։"
      , _O = "PG-ի պաշտոնական և իրական խաղեր՝ Askmebet-ի հետ"
      , hO = "Հաստատված է միայն Askmebet Thailand-ի կողմից։"
      , mO = "Ցանցային անկայուն կապ"
      , gO = "Թարմացրեք այս էջը և նորից փորձեք։"
      , bO = "Հաստատումն անավարտ է"
      , yO = "Նորից պտտեք և հաստատեք ամենավերջին գործարքի ID-ն։"
      , vO = "ԿԱՄ"
      , kO = "Խաղի պատմության էջում ընտրեք Գործարքի ID-ներից մեկը, որ նորից հաստատեք։"
      , AO = "Լիցենզավորված\nէ ՄԹ-ում և Մալթայում"
      , $O = "Վկայագրված է GA / BMM\n(UKGC)-ի կողմից"
      , TO = {
        official: YS,
        nofound: XS,
        nofound_details: JS,
        expired: QS,
        expired_details: ZS,
        wrong: eO,
        wrong_details: tO,
        error: nO,
        error_details: iO,
        fake: sO,
        fake_details: rO,
        gentle: oO,
        reminder: aO,
        certified: lO,
        step1: cO,
        step1_details: uO,
        step2: fO,
        step2_details: dO,
        chars: pO,
        official_th: _O,
        th_only: hO,
        unstable: mO,
        unstable_details: gO,
        incompleted: bO,
        incompleted_return: yO,
        incompleted_or: vO,
        incompleted_history: kO,
        licensed_top: AO,
        certified_top: $O
    }
      , EO = "Permainan PG Resmi dan Asli"
      , wO = "ID Transaksi tidak ditemukan"
      , SO = "Silakan kembali ke halaman riwayat permainan untuk memverifikasi catatan taruhan terakhir."
      , OO = "Sesi Kedaluwarsa"
      , PO = "Silakan kembali ke halaman riwayat permainan untuk memverifikasi catatan taruhan terakhir."
      , xO = "Parameter Salah"
      , IO = "Silakan kembali ke halaman riwayat permainan untuk memverifikasi catatan taruhan terakhir."
      , CO = "Kesalahan Server Internal"
      , RO = "Silakan muat ulang atau kembali ke halaman riwayat permainan untuk memverifikasi catatan taruhan terakhir."
      , LO = "PERHATIAN: Permainan Palsu Dideteksi!"
      , NO = "Permainan ini BUKAN produk resmi PG SOFT<sup>®</sup>! Kami sangat menyarankan agar Anda berhenti bermain segera untuk keamanan Anda."
      , DO = "Sekadar Pengingat"
      , MO = "Permainan palsu tidak disertifikasi dan dapat membawa risiko keamanan dan ancaman keuangan. Harap gunakan situs web verifikasi resmi {link} untuk memastikan bahwa hak-hak Anda terlindungi."
      , FO = "Produk-produk PG SOFT<sup>®</sup> disertifikasi ganda dan diuji ketat oleh otoritas global BMM dan GA, memastikan algoritma dan implementasi RNG, matematika permainan, dan aturannya memenuhi standar industri tertinggi demi keadilan & integritas."
      , GO = "LANGKAH 1:"
      , UO = "Pilih salah satu ID Transaksi."
      , jO = "LANGKAH 2:"
      , BO = "Klik tombol Verifikasi. Anda akan diarahkan secara otomatis."
      , zO = "* Pastikan URL tepat sama."
      , VO = "PG Games Resmi dan Asli dengan Askmebet"
      , HO = "Diverifikasi oleh Askmebet Thailand saja."
      , KO = "Koneksi Jaringan Tidak Stabil"
      , qO = "Harap segarkan halaman ini dan coba lagi."
      , WO = "Verifikasi Tidak Selesai"
      , YO = "Putar ulang dan verifikasi ID Transaksi terakhir."
      , XO = "ATAU"
      , JO = "Pilih salah satu ID Transaksi di halaman riwayat permainan untuk memverifikasi lagi."
      , QO = "Dilisensi di\nInggris Raya dan Malta"
      , ZO = "Disertifikasi oleh GA /\nBMM (UKGC)"
      , eP = {
        official: EO,
        nofound: wO,
        nofound_details: SO,
        expired: OO,
        expired_details: PO,
        wrong: xO,
        wrong_details: IO,
        error: CO,
        error_details: RO,
        fake: LO,
        fake_details: NO,
        gentle: DO,
        reminder: MO,
        certified: FO,
        step1: GO,
        step1_details: UO,
        step2: jO,
        step2_details: BO,
        chars: zO,
        official_th: VO,
        th_only: HO,
        unstable: KO,
        unstable_details: qO,
        incompleted: WO,
        incompleted_return: YO,
        incompleted_or: XO,
        incompleted_history: JO,
        licensed_top: QO,
        certified_top: ZO
    }
      , tP = "Giochi PG ufficiali e originali"
      , nP = "ID della transazione non trovato"
      , iP = "Ti preghiamo di tornare alla pagina della cronologia di gioco per verificare l’ultima registrazione delle puntate."
      , sP = "Sessione scaduta"
      , rP = "Ti preghiamo di tornare alla pagina della cronologia di gioco per verificare l’ultima registrazione delle puntate."
      , oP = "Parametro non corretto"
      , aP = "Ti preghiamo di tornare alla pagina della cronologia di gioco per verificare l’ultima registrazione delle puntate."
      , lP = "Errore interno del server"
      , cP = "Ti preghiamo di aggiornare la pagina o di tornare alla pagina della cronologia di gioco per verificare l’ultima registrazione delle puntate."
      , uP = "ATTENZIONE: Abbiamo rilevato un gioco contraffatto!"
      , fP = "Questo gioco NON è un prodotto ufficiale di PG SOFT<sup>®</sup>! Per la tua sicurezza, ti consigliamo di smettere di giocare immediatamente."
      , dP = "Promemoria"
      , pP = "I giochi contraffatti non sono certificati e possono comportare rischi per la sicurezza e minacce finanziarie. Ti preghiamo di utilizzare il sito web di verifica ufficiale per assicurarti che i tuoi diritti vengano tutelati: {link}"
      , _P = "I prodotti di PG SOFT<sup>®</sup> sono stati certificati e testati con rigore dalle autorità globali BMM e GA, assicurando che l’implementazione e l’algoritmo del RNG, i calcoli del gioco e le sue regole rispettino gli standard del settore."
      , hP = "1:"
      , mP = "Seleziona uno degli ID di transazione."
      , gP = "2:"
      , bP = "Fai clic sul Pulsante di verifica. Verrai reindirizzato/a."
      , yP = "* Assicurati che gli URL siano uguali."
      , vP = "Giochi PG ufficiali e originali con Askmebet"
      , kP = "Verificato solamente da Askmebet Thailand"
      , AP = "La connessione alla rete è instabile"
      , $P = "Ti preghiamo di aggiornare questa pagina e riprovare."
      , TP = "Verifica incompleta"
      , EP = "Rigira e verifica l’ID dell’ultima transazione."
      , wP = "OPPURE"
      , SP = "Seleziona uno degli ID delle transazioni presenti nella cronologia del gioco per verificarlo nuovamente."
      , OP = "In possesso di licenza nel Regno\nUnito e a Malta"
      , PP = "Certificato da GA /\nBMM (UKGC)"
      , xP = {
        official: tP,
        nofound: nP,
        nofound_details: iP,
        expired: sP,
        expired_details: rP,
        wrong: oP,
        wrong_details: aP,
        error: lP,
        error_details: cP,
        fake: uP,
        fake_details: fP,
        gentle: dP,
        reminder: pP,
        certified: _P,
        step1: hP,
        step1_details: mP,
        step2: gP,
        step2_details: bP,
        chars: yP,
        official_th: vP,
        th_only: kP,
        unstable: AP,
        unstable_details: $P,
        incompleted: TP,
        incompleted_return: EP,
        incompleted_or: wP,
        incompleted_history: SP,
        licensed_top: OP,
        certified_top: PP
    }
      , IP = "PGの公式・純正ゲーム"
      , CP = "トランザクションIDが見つかりませんでした"
      , RP = "最新のベット記録を検証するには、ゲーム履歴ページに戻ってください。"
      , LP = "セッション期限切れ"
      , NP = "最新のベット記録を検証するには、ゲーム履歴ページに戻ってください。"
      , DP = "不正確なパラメーター"
      , MP = "最新のベット記録を検証するには、ゲーム履歴ページに戻ってください。"
      , FP = "内部サーバーエラー"
      , GP = "最新のベット記録を検証するには、ゲーム履歴ページを更新するか、ゲーム履歴ページに戻ってください。"
      , UP = "注意：偽造ゲームが検出されました！"
      , jP = "このゲームはPG SOFT<sup>®</sup>の正規品ではありません！安全のため、直ちにプレイを中止することを強くお勧めします。"
      , BP = "注意喚起"
      , zP = "偽造ゲームは認証されておらず、セキュリティリスクや金銭的脅威をもたらす可能性があります。お客様の権利が確実に守られるよう、公式検証サイト{link}をご利用ください。"
      , VP = "世界的権威であるBMMおよびGAの厳格なデュアル認証およびテストを受けることにより、PG SOFT<sup>®</sup>製品は、RNGアルゴリズムと実装、ゲーム数学、ルールが公平性と完全性に関する業界最高基準を満たしていることを保証しています。"
      , HP = "ステップ1："
      , KP = "トランザクションIDを選択します。"
      , qP = "ステップ2："
      , WP = "検証ボタンをタップします。自動的にリダイレクトされます。"
      , YP = "* URLが正確かどうか確認してください。"
      , XP = "Askmebet提供のPGの公式・純正ゲーム"
      , JP = "Askmebetタイでのみ検証。"
      , QP = "不安定なネットワーク接続"
      , ZP = "このページを再読み込みして再度お試しください。"
      , ex = "検証が未完です"
      , tx = "再スピンして、最新のトランザクションIDを検証してください。"
      , nx = "または"
      , ix = "ゲーム履歴ページでいずれかのトランザクションIDを選択して、再度検証してください。"
      , sx = "英国とマルタで\nライセンス取得済み"
      , rx = "GA / \nBMMにより認可 (UKGC)"
      , ox = {
        official: IP,
        nofound: CP,
        nofound_details: RP,
        expired: LP,
        expired_details: NP,
        wrong: DP,
        wrong_details: MP,
        error: FP,
        error_details: GP,
        fake: UP,
        fake_details: jP,
        gentle: BP,
        reminder: zP,
        certified: VP,
        step1: HP,
        step1_details: KP,
        step2: qP,
        step2_details: WP,
        chars: YP,
        official_th: XP,
        th_only: JP,
        unstable: QP,
        unstable_details: ZP,
        incompleted: ex,
        incompleted_return: tx,
        incompleted_or: nx,
        incompleted_history: ix,
        licensed_top: sx,
        certified_top: rx
    }
      , ax = "공식 정품 PG 게임"
      , lx = "거래 ID를 찾을 수 없습니다"
      , cx = "게임 기록 페이지로 돌아가 최근 베팅 기록을 인증하시기 바랍니다."
      , ux = "세션 만료됨"
      , fx = "게임 기록 페이지로 돌아가 최근 베팅 기록을 인증하시기 바랍니다."
      , dx = "정확하지 않은 매개변수"
      , px = "게임 기록 페이지로 돌아가 최근 베팅 기록을 인증하시기 바랍니다."
      , _x = "내부 서버 오류"
      , hx = "새로고침 하거나 게임 기록 페이지로 돌아가 최근 베팅 기록을 인증하시기 바랍니다."
      , mx = "주의: 위조된 게임이 감지되었습니다!"
      , gx = "이 게임은 공식 PG SOFT<sup>®</sup> 제품이 아닙니다! 안전을 위해 즉시 게임을 중단하실 것을 강력히 권고드립니다."
      , bx = "안내문"
      , yx = "위조된 게임은 인증되지 않으며 보안상의 위험 및 금전적인 위협을 초래할 수 있습니다. 공식 인증 웹사이트인 {link}을 통해 귀하의 권리가 보호되고 있는지 확인하시기 바랍니다."
      , vx = "PG SOFT<sup>®</sup> 제품은 세계적으로 권위 있는 BMM 및 GA를 통해 엄격한 이중 인증과 테스트를 거쳐, RNG 알고리즘 및 구현, 게임 수학, 규칙이 업계에서 가장 높은 수준의 공정성 및 무결성 표준을 준수하도록 보장합니다."
      , kx = "1단계:"
      , Ax = "거래 ID 하나 를선택합니다."
      , $x = "2단계:"
      , Tx = "인증 버튼을 클릭합니다. 자동으로 리디렉션됩니다."
      , Ex = "* URL이 정확한지 확인하세요."
      , wx = "Askmebet과 함께하는 공식 정품 PG 게임"
      , Sx = "오직 Askmebet Thailand에서만 인증되었습니다."
      , Ox = "불안정한 네트워크 연결"
      , Px = "이 페이지를 새로 고침하고 다시 시도하세요."
      , xx = "인증 미완료"
      , Ix = "재스핀하고 최근 거래 ID를 인증하세요."
      , Cx = "또는"
      , Rx = "게임 기록 페이지에서 다시 인증할 거래 ID 하나를 선택하세요."
      , Lx = "영국 및 몰타에서 라\n이선스 보유"
      , Nx = "GA / \nBMM에 의해 인증 (UKGC)"
      , Dx = {
        official: ax,
        nofound: lx,
        nofound_details: cx,
        expired: ux,
        expired_details: fx,
        wrong: dx,
        wrong_details: px,
        error: _x,
        error_details: hx,
        fake: mx,
        fake_details: gx,
        gentle: bx,
        reminder: yx,
        certified: vx,
        step1: kx,
        step1_details: Ax,
        step2: $x,
        step2_details: Tx,
        chars: Ex,
        official_th: wx,
        th_only: Sx,
        unstable: Ox,
        unstable_details: Px,
        incompleted: xx,
        incompleted_return: Ix,
        incompleted_or: Cx,
        incompleted_history: Rx,
        licensed_top: Lx,
        certified_top: Nx
    }
      , Mx = "ເກມ PG ແທ້ ແລະ ເປັນທາງການ"
      , Fx = "ບໍ່ພົບລະຫັດທຸລະກໍາ"
      , Gx = "ກະລຸນາກັບໄປທີ່ໜ້າປະຫວັດເກມເພື່ອກວດສອບບັນທຶກການເດີມພັນຄັ້ງຫຼ້າສຸດ."
      , Ux = "ເຊັດຊັນຫມົດອາຍຸແລ້ວ"
      , jx = "ກະລຸນາກັບໄປທີ່ໜ້າປະຫວັດເກມເພື່ອກວດສອບບັນທຶກການເດີມພັນຄັ້ງຫຼ້າສຸດ."
      , Bx = "ປັດໄຈກໍານົດບໍ່ຖືກຕ້ອງ"
      , zx = "ກະລຸນາກັບໄປທີ່ໜ້າປະຫວັດເກມເພື່ອກວດສອບບັນທຶກການເດີມພັນຄັ້ງຫຼ້າສຸດ."
      , Vx = "ເກີດຂໍ້ພິດພາດເຊີບເວີພາຍໃນ"
      , Hx = "ກະລຸນາໂຫຼດຂໍ້ມູນຄືນໃໝ່ ຫຼື ກັບໄປທີ່ໜ້າປະຫວັດເກມເພື່ອກວດສອບບັນທຶກການເດີມພັນຄັ້ງຫຼ້າສຸດ."
      , Kx = "ຄໍາເຕືອນ: ກວດພົບເກມປອມ!"
      , qx = "ເກມນີ້ບໍ່ແມ່ນຜະລິດຕະພັນທີ່ເປັນທາງການຂອງ PG SOFT<sup>®</sup>! ພວກເຮົາຂໍແນະນໍາໃຫ້ທ່ານຢຸດຫຼິ້ນທັນທີ ເພື່ອຄວາມປອດໄພຂອງທ່ານເອງ."
      , Wx = "ຂໍອະນຸຍາດແຈ້ງເຕືອນ"
      , Yx = "ເກມທີ່ຮຽນແບບແມ່ນບໍ່ໄດ້ຮັບການຮັບຮອງ ແລະ ອາດຈະກໍ່ໃຫ້ເກີດຄວາມສ່ຽງດ້ານຄວາມປອດໄພ ແລະ ການຄຸກຄາມທາງດ້ານການເງິນ. ກະລຸນານໍາໃຊ້ເວັບໄຊ້ທາງການ {link} ເພື່ອໃຫ້ໝັ້ນໃຈວ່າສິດຂອງທ່ານໄດ້ຮັບການປົກປ້ອງ."
      , Xx = "ຜະລິດຕະພັນ PG SOFT<sup>®</sup> ໄດ້ຮັບການຮັບຮອງ ແລະ ທົດສອບຢ່າງເຂັ້ມງວດໂດຍ ໜ່ວຍງານ BMM ແລະ GA ລະດັບໂລກ, ເພື່ອໃຫ້ໝັ້ນໃຈວ່າ ສູດການຄິດໄລ່ RNG ແລະ ການນໍາໄປໃຊ້, ຄະນິດສາດໃນເກມ ແລະ ກົດລະບຽບ ເປັນໄປຕາມມາດຕະຖານອຸດສາຫະກໍາສູງສຸດ ເພື່ອຄວາມເປັນທໍາ ແລະ ຄວາມສົມບູນ."
      , Jx = "ຂັ້ນຕອນ 1:"
      , Qx = "ເລືອກໜຶ່ງໃນລະຫັດທຸລະກໍາ."
      , Zx = "ຂັ້ນຕອນ 2:"
      , eI = "ກົດທີ່ປຸ່ມກວດສອບ.ທ່ານຈະຖືກນໍາໄປໂດຍອັດຕະໂນມັດ."
      , tI = "* ກວດສອບໃຫ້ໝັ້ນໃຈວ່າ URL ກົງກັນທັງໝົດ."
      , nI = "ເກມ PG ແທ້ ແລະ ເປັນທາງການກັບ Askmebet"
      , iI = "ກວດສອບໂດຍ Askmebet ປະເທດໄທເທົ່ານັ້ນ."
      , sI = "ການເຊື່ອມຕໍ່ເຄືອຂ່າຍບໍ່ຄົງທີ່"
      , rI = "ກະລຸນາໂຫຼດໜ້ານີ້ຄືນໃໝ່ ແລະ ລອງໃໝ່ອີກຄັ້ງ."
      , oI = "ການກວດສອບບໍ່ຄົບຖ້ວນ"
      , aI = "ໝູນຄືນໃໝ່ ແລະ ກວດສອບລະຫັດທຸລະກໍາຫຼ້າສຸດ."
      , lI = "ຫຼື"
      , cI = "ເລືອກໜຶ່ງໃນລະຫັດທຸລະກໍາໃນໜ້າປະຫວັດເກມເພື່ອກວດສອບອີກຄັ້ງ."
      , uI = "ໄດ້ຮັບອະນຸຍາດ\nໃນສະຫະລາດຊະອາ\nນາຈັກ ແລະ ມອນຕ້າ"
      , fI = "ຢັ້ງຢືນໂດຍ \nGA / BMM (UKGC)"
      , dI = {
        official: Mx,
        nofound: Fx,
        nofound_details: Gx,
        expired: Ux,
        expired_details: jx,
        wrong: Bx,
        wrong_details: zx,
        error: Vx,
        error_details: Hx,
        fake: Kx,
        fake_details: qx,
        gentle: Wx,
        reminder: Yx,
        certified: Xx,
        step1: Jx,
        step1_details: Qx,
        step2: Zx,
        step2_details: eI,
        chars: tI,
        official_th: nI,
        th_only: iI,
        unstable: sI,
        unstable_details: rI,
        incompleted: oI,
        incompleted_return: aI,
        incompleted_or: lI,
        incompleted_history: cI,
        licensed_top: uI,
        certified_top: fI
    }
      , pI = "Oficialūs ir originalūs PG žaidimai"
      , _I = "Sandorio ID nerastas"
      , hI = "Grįžkite į žaidimo istorijos puslapį, kad patikrintumėte paskutinio statymo įrašą."
      , mI = "Prisijungimo laikas baigėsi"
      , gI = "Grįžkite į žaidimo istorijos puslapį, kad patikrintumėte paskutinio statymo įrašą."
      , bI = "Neteisingas parametras"
      , yI = "Grįžkite į žaidimo istorijos puslapį, kad patikrintumėte paskutinio statymo įrašą."
      , vI = "Vidinė serverio klaida"
      , kI = "Atnaujinkite arba grįžkite į žaidimo istorijos puslapį, kad patikrintumėte paskutinio statymo įrašą."
      , AI = "ATSARGIAI: aptiktas suklastotas žaidimas!"
      , $I = "Šis žaidimas NĖRA oficialus PG SOFT<sup>®</sup> produktas! Primygtinai rekomenduojame nedelsiant nutraukti žaidimą, kad užtikrintumėte savo saugumą."
      , TI = "Tik primename"
      , EI = "Padirbti žaidimai nėra sertifikuoti ir gali kelti finansinę grėsmę bei pavojų saugumui. Norėdami užtikrinti, kad jūsų teisės yra apsaugotos, naudokitės oficialia patvirtinimo svetaine {link}"
      , wI = "PG SOFT<sup>®</sup> produktus griežtai du kartus sertifikavo ir patikrino pasaulinės institucijos BMM ir GA, užtikrindamos, kad RNG algoritmas ir įgyvendinimas, žaidimo matematika ir taisyklės atitinka aukščiausius patikimumo ir sąžiningumo standartus šioje srityje."
      , SI = "1 VEIKSMAS"
      , OI = "Pasirinkite vieną iš sandorio ID."
      , PI = "2 VEIKSMAS"
      , xI = "Spustelėkite patvirtinimo mygtuką. Būsite automatiškai nukreipti."
      , II = "* Užtikrinkite, kad URL tiksliai atitiktų."
      , CI = "Oficialūs ir originalūs PG žaidimai su „Askmebet“"
      , RI = "Patikrinta tik „Askmebet Thailand“."
      , LI = "Nestabilus tinklo ryšys"
      , NI = "Atnaujinkite šį puslapį ir bandykite dar kartą."
      , DI = "Patvirtinimas nebaigtas"
      , MI = "Pakartotinai sukite ir patvirtinkite naujausio sandorio ID."
      , FI = "ARBA"
      , GI = "Žaidimo istorijos puslapyje pasirinkite vieną iš sandorio ID, kurį norite dar kartą patvirtinti."
      , UI = "Licencijuota Jungtinėje\nKaralystėje ir Maltoje"
      , jI = "Sertifikavo\nGA / BMM (UKGC)"
      , BI = {
        official: pI,
        nofound: _I,
        nofound_details: hI,
        expired: mI,
        expired_details: gI,
        wrong: bI,
        wrong_details: yI,
        error: vI,
        error_details: kI,
        fake: AI,
        fake_details: $I,
        gentle: TI,
        reminder: EI,
        certified: wI,
        step1: SI,
        step1_details: OI,
        step2: PI,
        step2_details: xI,
        chars: II,
        official_th: CI,
        th_only: RI,
        unstable: LI,
        unstable_details: NI,
        incompleted: DI,
        incompleted_return: MI,
        incompleted_or: FI,
        incompleted_history: GI,
        licensed_top: UI,
        certified_top: jI
    }
      , zI = "Албан ёсны, жинхэнэ PG тоглоомууд"
      , VI = "Гүйлгээний ID олдсонгүй"
      , HI = "Хамгийн сүүлийн мөрийний бүртгэлийг баталгаажуулахын тулд тоглоомын түүхийн хуудас руу буцна уу."
      , KI = "Үеийн хугацаа дууссан"
      , qI = "Хамгийн сүүлийн мөрийний бүртгэлийг баталгаажуулахын тулд тоглоомын түүхийн хуудас руу буцна уу."
      , WI = "Буруу параметр"
      , YI = "Хамгийн сүүлийн мөрийний бүртгэлийг баталгаажуулахын тулд тоглоомын түүхийн хуудас руу буцна уу."
      , XI = "Дотоод серверийн алдаа"
      , JI = "Хамгийн сүүлийн мөрийний бүртгэлийг баталгаажуулахын тулд тоглоомын түүхийн хуудсыг шинэчлэх эсвэл буцна уу."
      , QI = "АНХААРУУЛГА: Хуурамч тоглоом илэрсэн!"
      , ZI = "Энэ тоглоо албан ёсны PG SOFT<sup>®</sup> бүтээгдэхүүн БИШ! Бид танд аюулгүй байдлын үүднээс тоглохоо даруй зогсоохыг зөвлөж байна."
      , eC = "Нөхөрсөг сануулга"
      , tC = "Хуурамч тоглоомууд гэрчилгээгүй бөгөөд аюулгүй байдлын эрсдэл, санхүүгийн аюул учруулж болзошгүй. Өөрийн эрхийг хамгаалахын тулд {link} албан ёсны баталгаажуулах вэбсайтыг ашиглана уу."
      , nC = "BMM болон GA глобал удирдлагууд PG SOFT<sup>®</sup> бүтээгдэхүүнүүдэд хос гэрчилгээ олгож, туршиж үзсэн \nбөгөөд RNG алгоритм ба хэрэгжилт, тоглоомын математик, дүрмүүд шударга, хүртээмжтэй байдлын хувьд салбарын хамгийн өндөр стандартад нийцэж байгааг баталгаажуулсан."
      , iC = "АЛХАМ 1:"
      , sC = "Гүйлгээний ID-ны нэгийг сонгоно уу"
      , rC = "АЛХАМ 2:"
      , oC = "Баталгаажуулах товч дээр дарна уу. Таныг автоматаар дахин чиглүүлнэ:"
      , aC = "* URL яг тохирч буй эсэхийг шалгаарай."
      , lC = "Албан ёсны, Askmebet-тэй жинхэнэ PG тоглоомууд"
      , cC = "Зөвхөн Тайландын Askmebet-р баталгаажуулсан."
      , uC = "Тогтворгүй сүлжээний холболт"
      , fC = "Энэ хуудсыг сэргээгээд дахин оролдоно уу."
      , dC = "Баталгаажуулалт дуусаагүй байна"
      , pC = "Дахин эргүүлж, сүүлийн гүйлгээний ID-г баталгаажуулна уу."
      , _C = "ЭСВЭЛ"
      , hC = "Баталгаажуулахын тулд тоглоомын түүх хуудсан дахь Гүйлгээний ID-н аль нэгийг сонгоно уу."
      , mC = "Их Британи\nболон Мальтад\nлицензтэй"
      , gC = "GA / BMM\n(UKGC)-с\nгэрчилгээ олгосон"
      , bC = {
        official: zI,
        nofound: VI,
        nofound_details: HI,
        expired: KI,
        expired_details: qI,
        wrong: WI,
        wrong_details: YI,
        error: XI,
        error_details: JI,
        fake: QI,
        fake_details: ZI,
        gentle: eC,
        reminder: tC,
        certified: nC,
        step1: iC,
        step1_details: sC,
        step2: rC,
        step2_details: oC,
        chars: aC,
        official_th: lC,
        th_only: cC,
        unstable: uC,
        unstable_details: fC,
        incompleted: dC,
        incompleted_return: pC,
        incompleted_or: _C,
        incompleted_history: hC,
        licensed_top: mC,
        certified_top: gC
    }
      , yC = "တရားဝင်ပြီး စစ်မှန်သော PG ဂိမ်းများ"
      , vC = "ငွေလွှဲအိုင်ဒီကို မတွေ့ရပါ"
      , kC = "နောက်ဆုံး လောင်းကစားမှတ်တမ်းကို အတည်ပြုရန် ဂိမ်းမှတ်တမ်း စာမျက်နှာသို့ ပြန်သွားပါ။"
      , AC = "အခန်းကဏ္ဍ သက်တမ်းလွန်သွားပါပြီ"
      , $C = "နောက်ဆုံး လောင်းကစားမှတ်တမ်းကို အတည်ပြုရန် ဂိမ်းမှတ်တမ်း စာမျက်နှာသို့ ပြန်သွားပါ။"
      , TC = "ကန့်သတ်ချက် မှားယွင်းနေသည်"
      , EC = "နောက်ဆုံး လောင်းကစားမှတ်တမ်းကို အတည်ပြုရန် ဂိမ်းမှတ်တမ်း စာမျက်နှာသို့ ပြန်သွားပါ။"
      , wC = "အတွင်းပိုင်းဆာဗာ ချို့ယွင်းချက်"
      , SC = "နောက်ဆုံး လောင်းကစားမှတ်တမ်းကို အတည်ပြုရန် ဂိမ်းမှတ်တမ်း စာမျက်နှာသို့ ပြန်သွားပါ (သို့) ရီဖရက်ရှ်လုပ်ပါ။"
      , OC = "သတိပေးချက်- ဂိမ်းအတု တွေ့ရှိထားသည်!"
      , PC = "ဤဂိမ်းသည် PG SOFT<sup>®</sup> ၏ တရားဝင် ထုတ်ကုန် မဟုတ်ပါ! သင့်လုံခြုံရေးအတွက် ကစားနေရာမှ ချက်ချင်း ရပ်တန့်ရန် အခိုင်အမာ အကြံပြုပါသည်။"
      , xC = "ပြေပြစ်သော အသိပေးချက်"
      , IC = "ဂိမ်းအတူများသည် တရားမဝင်သည့်အပြင် လုံခြုံရေး နှင့် ငွေကြေး ခြိမ်းခြောက်မှု အန္တရာယ်များ ဖြစ်ပေါ်လာနိုင်သည်။ သင့်အကျိုးခံစားခွင့်များကို သေချာစွာ ကာကွယ်နိုင်ရန် တရားဝင် အတည်ပြုထားသော ဝဘ်ဆိုက် {link} ကို အသုံးပြုပါ။"
      , CC = "PG SOFT<sup>®</sup> ကို ကမ္ဘာ့အာဏာပိုင်အဖွဲ့ BMM နှင့် GA တို့က စစ်ဆေး၍၊ အသိအမှတ်ပြုထားသည်။ ၎င်းက RNG စနစ်၊စီမံဆောင်ရွက်ချက် နှင့် ဂိမ်း၏ ဂဏန်းသင်္ချာတို့ကို စီမံပေးသည့်အပြင်၊ စည်းမျဉ်းများမှာလည်းတရားမျှတသည့် အမြင့်ဆုံးစံညွှန်းများနှင့် ကိုက်ညီသည်။"
      , RC = "အဆင့် 1-"
      , LC = "ငွေအလွှဲအပြောင်းအိုင်ဒီထဲမှတစ်ခုကို‌ ရွေးပါ။"
      , NC = "အဆင့် 2-"
      , DC = "အတည်ပြုခလုတ်ကိုနှိပ်ပါ။သင့်အား အလိုအလျောက် ပြန်လည်ညွှန်ပြ‌ပေးမည်။"
      , MC = "* URL နှင့် အတိအကျ ကိုက်ညီမှုရှိအောင် ဆောင်ရွက်ပါ။"
      , FC = "Askmebet နှင့်အတူ တရားဝင်ပြီး စစ်မှန်သော PG ဂိမ်းများ"
      , GC = "Askmebet Thailand ကသာ အတည်ပြုထားသည်။"
      , UC = "အင်တာနက်လိုင်း မငြိမ်ပါ"
      , jC = "ဤစာမျက်နှာကို ရီဖရက်ရှ်လုပ်ပြီး၊ ထပ်ကြိုးစားကြည့်ပါ။"
      , BC = "အတည်ပြုချက် မပြီးဆုံးသေးပါ"
      , zC = "ပြန်လှည့်ပြီး၊ နောက်ဆုံး ငွေအလွှဲအပြောင်းအိုင်ဒီကို အတည်ပြုပါ။"
      , VC = "(သို့)"
      , HC = "ထပ်မံ အတည်ပြုရန် ဂိမ်းမှတ်တမ်းထဲရှိ ငွေအလွှဲအပြောင်းအိုင်ဒီထဲမှ တစ်ခုကို‌ ရွေးချယ်ပါ။"
      , KC = "ဗြိတိန်နှင့် မောလ်တာတွင်\nလိုင်စင်ရထားပြီးပါပြီ"
      , qC = "GA / BMM (UKGC)\nအသိအမှတ်ပြုဖြစ်သည်"
      , WC = {
        official: yC,
        nofound: vC,
        nofound_details: kC,
        expired: AC,
        expired_details: $C,
        wrong: TC,
        wrong_details: EC,
        error: wC,
        error_details: SC,
        fake: OC,
        fake_details: PC,
        gentle: xC,
        reminder: IC,
        certified: CC,
        step1: RC,
        step1_details: LC,
        step2: NC,
        step2_details: DC,
        chars: MC,
        official_th: FC,
        th_only: GC,
        unstable: UC,
        unstable_details: jC,
        incompleted: BC,
        incompleted_return: zC,
        incompleted_or: VC,
        incompleted_history: HC,
        licensed_top: KC,
        certified_top: qC
    }
      , YC = "Officiële en originele PG-spellen"
      , XC = "Transactie-ID niet gevonden"
      , JC = "Ga terug naar de pagina met de spelgeschiedenis om de laatste inzet te controleren."
      , QC = "Sessie verlopen"
      , ZC = "Ga terug naar de pagina met de spelgeschiedenis om de laatste inzet te controleren."
      , eR = "Onjuist parameter"
      , tR = "Ga terug naar de pagina met de spelgeschiedenis om de laatste inzet te controleren."
      , nR = "Interne serverfout"
      , iR = "Vernieuw of ga terug naar de pagina met de spelgeschiedenis om de laatste inzet te controleren."
      , sR = "LET OP: Namaakspel gedetecteerd!"
      , rR = "Dit spel is GEEN officieel product van PG SOFT<sup>®</sup> We raden je ten zeerste aan om te stoppen met spelen. Dit is voor je eigen veiligheid."
      , oR = "Vriendelijke herinnering"
      , aR = "Namaakspellen zijn niet gecertificeerd en kunnen beveiligingsrisico's en financiële bedreigingen vormen. Gebruik de officiële verificatiewebsite {link} om er zeker van te zijn dat je rechten beschermd zijn."
      , lR = "De autoriteiten BMM en GA certificeren en testen de producten van PG SOFT<sup>®</sup> extra streng. Zo voldoen spelberekeningen en regels aan de hoogste integriteitsnormen, evenals het RNG-algoritme en de implementatie."
      , cR = "STAP 1:"
      , uR = "Kies een van de Transactie-ID's."
      , fR = "STAP 2:"
      , dR = "Klik op de knop voor verificatie. Je wordt automatisch omgeleid."
      , pR = "* Zorg dat de URL exact overeenkomt."
      , _R = "Officiële en originele PG-spellen met Askmebet"
      , hR = "Alleen geverifieerd door Askmebet Thailand."
      , mR = "Instabiele netwerkverbinding"
      , gR = "Vernieuw deze pagina en probeer het opnieuw."
      , bR = "Onvolledige verificatie"
      , yR = "Respin en verifieer de laatste transactie-ID."
      , vR = "OF"
      , kR = "Selecteer een van de transactie-ID's op de pagina met de spelgeschiedenis om opnieuw te verifiëren."
      , AR = "Gelicentieerd in\nhet VK en Malta"
      , $R = "Gecertificeerd door GA /\nBMM (UKGC)"
      , TR = {
        official: YC,
        nofound: XC,
        nofound_details: JC,
        expired: QC,
        expired_details: ZC,
        wrong: eR,
        wrong_details: tR,
        error: nR,
        error_details: iR,
        fake: sR,
        fake_details: rR,
        gentle: oR,
        reminder: aR,
        certified: lR,
        step1: cR,
        step1_details: uR,
        step2: fR,
        step2_details: dR,
        chars: pR,
        official_th: _R,
        th_only: hR,
        unstable: mR,
        unstable_details: gR,
        incompleted: bR,
        incompleted_return: yR,
        incompleted_or: vR,
        incompleted_history: kR,
        licensed_top: AR,
        certified_top: $R
    }
      , ER = "Offisielle og ekte PG-spill"
      , wR = "Transaksjons-ID ikke funnet"
      , SR = "Gå tilbake til spillhistorikksiden for å verifisere din forrige innsatsrekord."
      , OR = "Økten har utløpt"
      , PR = "Gå tilbake til spillhistorikksiden for å verifisere din forrige innsatsrekord."
      , xR = "Feil parameter"
      , IR = "Gå tilbake til spillhistorikksiden for å verifisere din forrige innsatsrekord."
      , CR = "Intern serverfeil"
      , RR = "Oppdater eller gå tilbake til spillhistorikksiden for å verifisere din forrige innsatsrekord."
      , LR = "ADVARSEL: Forfalsket spill oppdaget!"
      , NR = "Dette spillet er IKKE et offisielt PG SOFT<sup>®</sup>-produkt! Vi anbefaler deg på det sterkeste å slutte å spille øyeblikkelig for din egen sikkerhet."
      , DR = "Vennlig påminnelse"
      , MR = "Forfalskede spill er ikke sertifiserte og kan utgjøre sikkerhetsrisikoer og økonomiske trusler. Bruk det offisielle verifiseringsnettstedet {link} for å sikre at rettighetene dine beskyttes."
      , FR = "PG SOFT<sup>®</sup>-produkter er strengt dobbeltsertifisert og testet av globale myndigheter, BMM og GA,\nsom sikrer at RNG-algoritmen og implementering, spillmatematikk og regler oppfyller\ntoppbransjestandarder for rettferdighet og integritet."
      , GR = "TRINN 1:"
      , UR = "Velg én av Transaksjons-ID."
      , jR = "TRINN 2:"
      , BR = "Klikk på Verifiser-knappen. Du blir omdirigert automatisk."
      , zR = "* Sørg for at nettadressen samsvarer nøyaktig."
      , VR = "Offisielle og ekte PG-spill med Askmebet"
      , HR = "Bare verifisert av Askmebet Thailand."
      , KR = "Ustabil nettverksforbindelse"
      , qR = "Oppdater denne siden og prøv igjen."
      , WR = "Verifiseringen er ufullstendig"
      , YR = "Respinn og verifisere den nyeste transaksjons-ID-en."
      , XR = "ELLER"
      , JR = "Velg én av transaksjons-ID-ene på spillhistorikksiden for å verifisere den igjen."
      , QR = "Lisensiert i\nStorbritannia og Malta"
      , ZR = "Sertifisert av GA /\nBMM (UKGC)"
      , eL = {
        official: ER,
        nofound: wR,
        nofound_details: SR,
        expired: OR,
        expired_details: PR,
        wrong: xR,
        wrong_details: IR,
        error: CR,
        error_details: RR,
        fake: LR,
        fake_details: NR,
        gentle: DR,
        reminder: MR,
        certified: FR,
        step1: GR,
        step1_details: UR,
        step2: jR,
        step2_details: BR,
        chars: zR,
        official_th: VR,
        th_only: HR,
        unstable: KR,
        unstable_details: qR,
        incompleted: WR,
        incompleted_return: YR,
        incompleted_or: XR,
        incompleted_history: JR,
        licensed_top: QR,
        certified_top: ZR
    }
      , tL = "Oficjalne i oryginalne PG"
      , nL = "Nie znaleziono ID transakcji"
      , iL = "Wróć na stronę historii gier, aby zweryfikować ostatni zarejestrowany zakład."
      , sL = "Sesja wygasła"
      , rL = "Wróć na stronę historii gier, aby zweryfikować ostatni zarejestrowany zakład."
      , oL = "Nieprawidłowy parametr"
      , aL = "Wróć na stronę historii gier, aby zweryfikować ostatni zarejestrowany zakład."
      , lL = "Wewnętrzny błąd serwera"
      , cL = "Odśwież lub wróć na stronę historii gier, aby zweryfikować ostatni zarejestrowany zakład."
      , uL = "UWAGA: wykryto fałszywą grę!"
      , fL = "Ta gra NIE jest oficjalnym produktem PG SOFT<sup>®</sup>! W trosce o Twoje bezpieczeństwo zalecamy natychmiastowe przerwanie gry."
      , dL = "Przypomnienie"
      , pL = "Nieoryginalne gry nie są certyfikowane – gra w takie gry może się wiązać z zagrożeniami w zakresach bezpieczeństwa oraz finansowym. Skorzystaj z oficjalnej strony weryfikacyjnej {link}, aby się upewnić, że Twoje prawa są chronione."
      , _L = "Produkty PG SOFT<sup>®</sup> zostały przetestowane przez globalne organy BMM oraz GA i otrzymały certyfikaty\npotwierdzające, że algorytm oraz implementacja RNG, matematyka gier i ich zasady są zgodne\nz najwyższymi standardami uczciwości i integralności."
      , hL = "KROK 1.:"
      , mL = "Wybierz jedno z ID transakcji."
      , gL = "KROK 2.:"
      , bL = "Kliknij przycisk Weryfikuj. Nastąpi automatyczne przekierowanie."
      , yL = "* Upewnij się, że adresy URL są takie same."
      , vL = "Oficjalne i oryginalne gry PG z Askmebet"
      , kL = "Zweryfikowane tylko przez Askmebet Tajlandia."
      , AL = "Niestabilne Połączenie Internetowe"
      , $L = "Odśwież tę stronę i spróbuj ponownie."
      , TL = "Weryfikacja Niedokończona"
      , EL = "Zakręć ponownie i zweryfikuj ostatnie ID Transakcji."
      , wL = "LUB"
      , SL = "Wybierz na stronie historii gier dowolne ID Transakcji, aby zweryfikować ponownie."
      , OL = "Działalność licencjonowana\nw Wielkiej Brytanii i na Malcie"
      , PL = "Certyfikat GA /\nBMM (UKGC)"
      , xL = {
        official: tL,
        nofound: nL,
        nofound_details: iL,
        expired: sL,
        expired_details: rL,
        wrong: oL,
        wrong_details: aL,
        error: lL,
        error_details: cL,
        fake: uL,
        fake_details: fL,
        gentle: dL,
        reminder: pL,
        certified: _L,
        step1: hL,
        step1_details: mL,
        step2: gL,
        step2_details: bL,
        chars: yL,
        official_th: vL,
        th_only: kL,
        unstable: AL,
        unstable_details: $L,
        incompleted: TL,
        incompleted_return: EL,
        incompleted_or: wL,
        incompleted_history: SL,
        licensed_top: OL,
        certified_top: PL
    }
      , IL = "Jogos PG Oficiais e Genuínos"
      , CL = "A ID da transação não foi encontrada"
      , RL = "Por favor, regresse à página do histórico do jogo para verificar o registo da aposta mais recente."
      , LL = "Sessão Expirada"
      , NL = "Por favor, regresse à página do histórico do jogo para verificar o registo da aposta mais recente."
      , DL = "Parâmetro Incorreto"
      , ML = "Por favor, regresse à página do histórico do jogo para verificar o registo da aposta mais recente."
      , FL = "Erro Interno do Servidor"
      , GL = "Por favor, atualize ou regresse à página do histórico do jogo para verificar o registo da aposta mais recente."
      , UL = "ATENÇÃO: Foi Detetado um Jogo Contrafeito!"
      , jL = "Este jogo NÃO é um produto oficial da PG SOFT<sup>®</sup>! Recomendamos vivamente que pare de jogar imediatamente para sua segurança."
      , BL = "Lembrete Amigável"
      , zL = "Os jogos contrafeitos não são certificados e podem colocar riscos de segurança e ameaças financeiras. Por favor, utilize o site oficial de verificação {link} para garantir que os seus direitos são protegidos."
      , VL = "Os produtos PG SOFT<sup>®</sup> foram certificados e testados pela BMM e a GA, garantindo que o algoritmo\ne a implementação do RNG, a matemática do jogo e as regras cumpram os padrões mais\nelevados do setor para imparcialidade e integridade."
      , HL = "PASSO 1:"
      , KL = "Selecione uma das ID da Transação."
      , qL = "PASSO 2:"
      , WL = "Clique no botão Verificar. Será redirecionado automaticamente."
      , YL = "* Certifique-se de que a URL corresponda exatamente."
      , XL = "Jogos PG Oficiais e Genuínos com a Askmebet"
      , JL = "Verificado apenas pela Askmebet Tailândia."
      , QL = "Conexão de Rede Instável"
      , ZL = "Atualize esta página e tente novamente."
      , eN = "Verificação Incompleta"
      , tN = "Faça o respin e verifique a ID de Transação mais recente."
      , nN = "OU"
      , iN = "Selecione uma das IDs de transação na página de histórico do jogo para verificar novamente."
      , sN = "Licenciado no Reino Unido \ne Malta"
      , rN = "Certificado por GA /\nBMM (UKGC)"
      , oN = {
        official: IL,
        nofound: CL,
        nofound_details: RL,
        expired: LL,
        expired_details: NL,
        wrong: DL,
        wrong_details: ML,
        error: FL,
        error_details: GL,
        fake: UL,
        fake_details: jL,
        gentle: BL,
        reminder: zL,
        certified: VL,
        step1: HL,
        step1_details: KL,
        step2: qL,
        step2_details: WL,
        chars: YL,
        official_th: XL,
        th_only: JL,
        unstable: QL,
        unstable_details: ZL,
        incompleted: eN,
        incompleted_return: tN,
        incompleted_or: nN,
        incompleted_history: iN,
        licensed_top: sN,
        certified_top: rN
    }
      , aN = "Jocuri PG originale și autentice"
      , lN = "ID Tranzacție nu a fost găsit"
      , cN = "Reveniți la pagina de istoric al jocului pentru a verifica ultima înregistrare a pariului."
      , uN = "Sesiune expirată"
      , fN = "Reveniți la pagina de istoric al jocului pentru a verifica ultima înregistrare a pariului."
      , dN = "Parametru incorect"
      , pN = "Reveniți la pagina de istoric al jocului pentru a verifica ultima înregistrare a pariului."
      , _N = "Eroare de server intern"
      , hN = "Reîncărcați pagina sau reveniți la pagina de istoric al jocului pentru a verifica ultima înregistrare a pariului."
      , mN = "ATENȚIE: S-a detectat un joc contrafăcut!"
      , gN = "Acest joc NU este un produs oficial PG SOFT<sup>®</sup>! Vă recomandăm să opriți imediat jocul, pentru siguranța dumneavoastră."
      , bN = "Rețineți:"
      , yN = "Jocurile contrafăcute nu sunt certificate și pot prezenta riscuri de securitate și amenințări financiare. Vă rugăm să utilizați site-ul oficial de verificare {link} pentru a vă asigura că drepturile dumneavoastră sunt protejate."
      , vN = "Produsele PG SOFT<sup>®</sup> au fost dublu certificate și testate de BMM și GA, asigurându-se, astfel, \ncă algoritmul și implementarea RNG, matematica jocului și regulile respectă cele mai \nînalte standarde legate de echitate și integritate."
      , kN = "PASUL 1:"
      , AN = "Selectați un ID Tranzacție."
      , $N = "PASUL 2:"
      , TN = "Dați click pe butonul Verificare. Veți fi redirecționat automat."
      , EN = "* Asigurați-vă că URL-ul este identic."
      , wN = "Jocuri PG originale și autentice cu Askmebet"
      , SN = "Doar Verificat de Askmebet Thailanda."
      , ON = "Conexiune la rețea instabilă"
      , PN = "Reîmprospătați pagina și reîncercați."
      , xN = "Verificare incompletă"
      , IN = "Reînvârtiți și verificați cel mai recent ID Tranzacție."
      , CN = "SAU"
      , RN = "Selectați un ID Tranzacție din pagina de istoric al jocului pentru a verifica din nou."
      , LN = "Licențiată în\nRegatul Unit și Malta"
      , NN = "Certificat de GA /\nBMM (UKGC)"
      , DN = {
        official: aN,
        nofound: lN,
        nofound_details: cN,
        expired: uN,
        expired_details: fN,
        wrong: dN,
        wrong_details: pN,
        error: _N,
        error_details: hN,
        fake: mN,
        fake_details: gN,
        gentle: bN,
        reminder: yN,
        certified: vN,
        step1: kN,
        step1_details: AN,
        step2: $N,
        step2_details: TN,
        chars: EN,
        official_th: wN,
        th_only: SN,
        unstable: ON,
        unstable_details: PN,
        incompleted: xN,
        incompleted_return: IN,
        incompleted_or: CN,
        incompleted_history: RN,
        licensed_top: LN,
        certified_top: NN
    }
      , MN = "Официальные и оригинальные игры PG"
      , FN = "ID транзакции не найден"
      , GN = "Вернись на страницу истории игры, чтобы посмотреть последнюю запись ставки."
      , UN = "Срок действия сеанса истек"
      , jN = "Вернись на страницу истории игры, чтобы посмотреть последнюю запись ставки."
      , BN = "Неправильный параметр"
      , zN = "Вернись на страницу истории игры, чтобы посмотреть последнюю запись ставки."
      , VN = "Внутренняя ошибка сервера"
      , HN = "Обновите или вернитесь на страницу истории игры, чтобы посмотреть последнюю запись ставки."
      , KN = "ВНИМАНИЕ: Выявлена поддельная игра!"
      , qN = "Эта игра НЕ является официальным продуктом PG SOFT<sup>®</sup>! Настоятельно рекомендуем незамедлительно прекратить игру ради вашей безопасности."
      , WN = "Внимание!"
      , YN = "Поддельные игры не сертифицированы и могут нести риск для безопасности и финансовые потери. Для обеспечения защиты ваших прав используйте официальный сайт проверки {link}."
      , XN = "Продукты PG SOFT<sup>®</sup> протестированы и сертифицированы международными органами BMM и GA.\nЭто гарантирует, что алгоритм и реализация ГСЧ, математические модели и правила игр\nсоответствуют самым строгим отраслевым стандартам."
      , JN = "ШАГ 1:"
      , QN = "Выберите один из ID транзакции."
      , ZN = "ШАГ 2:"
      , eD = "Нажмите кнопку «Верифицировать». Произойдет автоматическая переадресация."
      , tD = "* Убедитесь, что URL-адрес в точности совпадает."
      , nD = "Официальные и оригинальные игры PG с Askmebet"
      , iD = "Верифицировано только Askmebet Thailand."
      , sD = "Нестабильное подключение к сети"
      , rD = "Обновите эту страницу и повторите попытку."
      , oD = "Верификация не завершена"
      , aD = "Вращайте повторно и верифицируйте последний ID транзакции."
      , lD = "ИЛИ"
      , cD = "Выберите один из ID транзакции на странице истории игры, чтобы верифицировать снова."
      , uD = "Лицензия получена в\nВеликобритании и на Мальте"
      , fD = "Сертификат GA /\nBMM (UKGC)"
      , dD = {
        official: MN,
        nofound: FN,
        nofound_details: GN,
        expired: UN,
        expired_details: jN,
        wrong: BN,
        wrong_details: zN,
        error: VN,
        error_details: HN,
        fake: KN,
        fake_details: qN,
        gentle: WN,
        reminder: YN,
        certified: XN,
        step1: JN,
        step1_details: QN,
        step2: ZN,
        step2_details: eD,
        chars: tD,
        official_th: nD,
        th_only: iD,
        unstable: sD,
        unstable_details: rD,
        incompleted: oD,
        incompleted_return: aD,
        incompleted_or: lD,
        incompleted_history: cD,
        licensed_top: uD,
        certified_top: fD
    }
      , pD = "නිල සහ නියම PG ක්‍රීඩා"
      , _D = "ගනුදෙනු ID හමු ‍නොවිය"
      , hD = "නවතම ඔට්ටු වාර්තාව සත්‍යාපනය කිරීමට කරුණාකර ක්‍රීඩා ඉතිහාස පිටුවට ආපසු යන්න."
      , mD = "සැසිය අවසන් විය"
      , gD = "නවතම ඔට්ටු වාර්තාව සත්‍යාපනය කිරීමට කරුණාකර ක්‍රීඩා ඉතිහාස පිටුවට ආපසු යන්න."
      , bD = "වැරදි පරාමිතිය"
      , yD = "නවතම ඔට්ටු වාර්තාව සත්‍යාපනය කිරීමට කරුණාකර ක්‍රීඩා ඉතිහාස පිටුවට ආපසු යන්න."
      , vD = "අභ්‍යන්තර ‍සේවාදායක ‍දෝෂය"
      , kD = "නවතම ඔට්ටු වාර්තාව සත්‍යාපනය කිරීමට කරුණාකර ක්‍රීඩා ඉතිහාස පිටුව නැවුම් කරන්න හෝ පිටුවට ආපසු යන්න."
      , AD = "අවවාදයයි: ව්‍යාජ ක්‍රීඩාවක් අනාවරණය විය!"
      , $D = "මෙම ක්‍රීඩාව PG SOFT<sup>®</sup> හි නිල නිෂ්පාදනයක් නොවේ! ඔබේ ආරක්ෂාව සඳහා වහාම ක්‍රීඩා කිරීම නවත්වන ලෙස අපි ඔබට තරයේ අවවාද කරමු."
      , TD = "කාරුණික මතක් කිරීම"
      , ED = "ව්‍යාජ ක්‍රීඩා සහතික කර නොමැති අතර ආරක්ෂක අවදානම් සහ මූල්‍ය තර්ජන ඇති කළ හැකි ය. ඔබේ අයිතීන් සුරක්ෂිත බව සහතික කිරීමට කරුණාකර නිල සත්‍යාපන වෙබ් අඩවිය වන {link} භාවිත කරන්න."
      , wD = "PG SOFT<sup>®</sup> නිෂ්පාදන RNG ඇල්ගොරිතම සහ ක්‍රියාත්මක කිරීම, ක්‍රීඩා ගණිතය සහ නීති \nසාධාරණත්වය සහ අඛණ්ඩතාව සඳහා ඉහළම කර්මාන්ත ප්‍රමිතීන් සපුරාලන බව සහතික කර \nගෝලීය අධිකාරීන් BMM සහ GA විසින් දැඩි ලෙස ද්විත්ව සහතික කර පරීක්ෂා කර ඇත."
      , SD = "අදියර 1:"
      , OD = "‍තෝරන්න ගනුදෙනු ID."
      , PD = "අදියර 2:"
      , xD = "ක්ලික් කරන්න ස්ථිර ‍බොත්තම. ස්වයංව ‍යොමු ‍කෙ‍රේ."
      , ID = "* URL නිවැරදිව ගැළපීම සහතික කරන්න."
      , CD = "Askmebet සමග නිල සහ නියම PG ක්‍රීඩා"
      , RD = "Askmebet Thailand විසින් පමණක් සත්‍යාපනය කර ඇත."
      , LD = "අස්ථායී ජාල සම්බන්ධතාවය"
      , ND = "කරුණාකර මෙම පිටුව නැවුම් කර නැවත උත්සාහ කරන්න."
      , DD = "සත්‍යාපනය අසාර්ථකයි"
      , MD = "නැවත කරකවා නවතම ගනුදෙනු ID ය සත්‍යාපනය කරන්න."
      , FD = "හෝ"
      , GD = "නැවත සත්‍යාපනය කිරීමට ක්‍රීඩා ඉතිහාස පිටුව තුළ ගනුදෙනු ID වලින් එකක් තෝරන්න."
      , UD = "එක්සත් රාජධානියේ\nහා මෝල්ටාවේ බලපත්"
      , jD = "GA / BMM (UKGC)\nසහතික කර ඇත"
      , BD = {
        official: pD,
        nofound: _D,
        nofound_details: hD,
        expired: mD,
        expired_details: gD,
        wrong: bD,
        wrong_details: yD,
        error: vD,
        error_details: kD,
        fake: AD,
        fake_details: $D,
        gentle: TD,
        reminder: ED,
        certified: wD,
        step1: SD,
        step1_details: OD,
        step2: PD,
        step2_details: xD,
        chars: ID,
        official_th: CD,
        th_only: RD,
        unstable: LD,
        unstable_details: ND,
        incompleted: DD,
        incompleted_return: MD,
        incompleted_or: FD,
        incompleted_history: GD,
        licensed_top: UD,
        certified_top: jD
    }
      , zD = "Oficiálne a pravé hry PG"
      , VD = "ID transakcie nebolo nájdené"
      , HD = "Vráťte sa prosím na stránku predošlých hier, aby ste overili najnovší stávkový záznam."
      , KD = "Relácia vypršala"
      , qD = "Vráťte sa prosím na stránku predošlých hier, aby ste overili najnovší stávkový záznam."
      , WD = "Nesprávny parameter"
      , YD = "Vráťte sa prosím na stránku predošlých hier, aby ste overili najnovší stávkový záznam."
      , XD = "Interná chyba servera"
      , JD = "Aktualizujte stránku alebo sa vráťte na stránku predošlých hier, aby ste overili najnovší stávkový záznam."
      , QD = "UPOZORNENIE: Bolo zistené, že hra je falošná!"
      , ZD = "Táto hra NIE JE oficiálnym produktom spoločnosti PG SOFT<sup>®</sup>! V záujme vašej bezpečnosti vám dôrazne odporúčame, aby ste hru okamžite ukončili."
      , eM = "Jemné pripomenutie"
      , tM = "Falšované hry nie sú certifikované a môžu predstavovať riziko pre bezpečnosť a finančnú hrozbu. Na zabezpečenie ochrany svojich práv použite oficiálnu overovaciu webovú stránku {link}."
      , nM = "Produkty PG SOFT<sup>®</sup> boli dôkladne duálne certifikované a testované globálnymi autoritami BMM a GA, ktoré zaručujú, že algoritmus RNG a jeho implementácia, herná matematika a pravidlá spĺňajú najprísnejšie priemyselné normy pre spravodlivosť a integritu."
      , iM = "KROK 1:"
      , sM = "Vyberte jedno z ID transakcie."
      , rM = "KROK 2:"
      , oM = "Kliknite na Tlačidlo overiť. Budete automaticky presmerovaný/á."
      , aM = "* Uistite sa, že URL adresa sa presne zhoduje."
      , lM = "Oficiálne a pravé hry PG so spoločnosťou Askmebet"
      , cM = "Overené iba spoločnosťou Askmebet Thailand."
      , uM = "Nestabilné sieťové pripojenie"
      , fM = "Obnovte túto stránku a skúste to znova."
      , dM = "Overenie je nedokončené"
      , pM = "Spustite opakované točenie a overte najnovšie ID transakcie."
      , _M = "ALEBO"
      , hM = "Na stránke histórie hry vyberte jedno z ID transakcie, ktoré sa má znova overiť."
      , mM = "Licencované vo\nVeľkej Británii a na Malte"
      , gM = "Certifikované\nGA / BMM (UKGC)"
      , bM = {
        official: zD,
        nofound: VD,
        nofound_details: HD,
        expired: KD,
        expired_details: qD,
        wrong: WD,
        wrong_details: YD,
        error: XD,
        error_details: JD,
        fake: QD,
        fake_details: ZD,
        gentle: eM,
        reminder: tM,
        certified: nM,
        step1: iM,
        step1_details: sM,
        step2: rM,
        step2_details: oM,
        chars: aM,
        official_th: lM,
        th_only: cM,
        unstable: uM,
        unstable_details: fM,
        incompleted: dM,
        incompleted_return: pM,
        incompleted_or: _M,
        incompleted_history: hM,
        licensed_top: mM,
        certified_top: gM
    }
      , yM = "Lojërat zyrtare dhe origjinale të PG"
      , vM = "ID e transaksionit nuk u gjet"
      , kM = "Ju lutemi kthehuni në faqen e historikut të lojës për të verifikuar regjistrimet më të fundit të basteve."
      , AM = "Sesioni Skadoi"
      , $M = "Ju lutemi kthehuni në faqen e historikut të lojës për të verifikuar regjistrimet më të fundit të basteve."
      , TM = "Parametër i Pasaktë"
      , EM = "Ju lutemi kthehuni në faqen e historikut të lojës për të verifikuar regjistrimet më të fundit të basteve."
      , wM = "Gabim i Brendshëm Serveri"
      , SM = "Ju lutemi rifreskoni ose kthehuni në faqen e historikut të lojës për të verifikuar regjistrimet më të fundit të basteve."
      , OM = "KUJDES: U identifikua lojë e falsifikuar!"
      , PM = "Kjo lojë NUK është produkt zyrtar i PG SOFT<sup>®</sup>! Për sigurinë tuaj, ju këshillojmë fuqishëm të ndaloni lojën menjëherë."
      , xM = "Përkujtim Xhentil"
      , IM = "Lojërat e falsifikuara nuk janë të certifikuara dhe mund të paraqesin rreziqe sigurie dhe kërcënime financiare. Ju lutemi përdorni faqen zyrtare të verifikimit {link} për të siguruar që të drejtat tuaja janë të mbrojtura."
      , CM = "Produktet PG SOFT<sup>®</sup> janë certifikuar dhe testuar nga dy autoritetet globale si BMM dhe GA, duke siguruar që algoritmi RNG dhe zbatimi, matematika dhe rregullat e lojës përmbushin standardet më të larta\ntë industrisë për ndershmëri dhe integritet."
      , RM = "HAPI 1:"
      , LM = "Zgjidh një nga ID-të e transaksionit."
      , NM = "HAPI 2:"
      , DM = "Kliko butonin Verifiko. Ju do të ridrejtoheni."
      , MM = "* Sigurohuni që URL-ja të përputhet saktësisht."
      , FM = "Lojërat zyrtare dhe origjinale të PG me Askmebet"
      , GM = "Verifikuar vetëm nga Askmebet Tajlandë."
      , UM = "Lidhje e paqëndrueshme e rrjetit"
      , jM = "Rifreskoni këtë faqe dhe provoni sërish."
      , BM = "Verifikimi jo i plotë"
      , zM = "Rirrotullo dhe verifiko ID-në e transaksionit më të fundit."
      , VM = "OSE"
      , HM = "Zgjidhni një nga ID-të e transaksionit në faqen e historikut të lojës për të verifikuar përsëri."
      , KM = "Licencuar në Mbretërinë \ne Bashkuar dhe në Maltë"
      , qM = "Certifikuar nga \nGA / BMM (UKGC)"
      , WM = {
        official: yM,
        nofound: vM,
        nofound_details: kM,
        expired: AM,
        expired_details: $M,
        wrong: TM,
        wrong_details: EM,
        error: wM,
        error_details: SM,
        fake: OM,
        fake_details: PM,
        gentle: xM,
        reminder: IM,
        certified: CM,
        step1: RM,
        step1_details: LM,
        step2: NM,
        step2_details: DM,
        chars: MM,
        official_th: FM,
        th_only: GM,
        unstable: UM,
        unstable_details: jM,
        incompleted: BM,
        incompleted_return: zM,
        incompleted_or: VM,
        incompleted_history: HM,
        licensed_top: KM,
        certified_top: qM
    }
      , YM = "Zvanične i Prave PG igre"
      , XM = "ID transakcije nije pronađen"
      , JM = "Molimo vas, vratite se na stranicu istorije igre kako biste verifikovali poslednji rekord uloga."
      , QM = "Sesija je Istekla"
      , ZM = "Molimo vas, vratite se na stranicu istorije igre kako biste verifikovali poslednji rekord uloga."
      , eF = "Neispravan parametar"
      , tF = "Molimo vas, vratite se na stranicu istorije igre kako biste verifikovali poslednji rekord uloga."
      , nF = "Unutrašnja Greška Servera."
      , iF = "Molimo vas, osvežite ili se vratite na stranicu istorije igre kako biste verifikovali poslednji rekord uloga."
      , sF = "OPREZ: Otkrivena Je Falsifikovana Igra"
      , rF = "Ova igra NIJE zvanični proizvod PG SOFT-a® Savetujemo vam da radi vaše bezbednosti odmah prestanete da je igrate."
      , oF = "Prijateljski podsetnik"
      , aF = "Falsifikovane igre nisu sertifikovane i mogu predstavljati bezbednosni rizik i finansijsku opasnost. Molimo vas, koristite zvanični veb-sajt za verifikaciju {link} kako biste bili sigurni da su vaša prava zaštićena."
      , lF = "PG SOFT<sup>®</sup> proizvodi su strogo dvostruko sertifikovani i testirani od strane svetskih autoriteta BMM i GA, kako bi RNG algoritam, matematika u igri i pravila zadovoljili najviše standarde poštene igre i integriteta u ovoj industriji."
      , cF = "KORAK 1:"
      , uF = "Odaberi jedan od ID-ova transakcije."
      , fF = "KORAK 2:"
      , dF = "Klikni na Taster potvrde. Bićeš preusmeren"
      , pF = "* Pobrinite se za to da se URL potpuno podudara."
      , _F = "Zvanične i Prave PG igre uz Askmebet"
      , hF = "Verifikovao Askmebet, samo u Tajlandu."
      , mF = "Nestabilna Veza Mreže"
      , gF = "Molimo vas, osvežite ovu stranici i pokušajte ponovo."
      , bF = "Verifikacija je Nedovršena"
      , yF = "Ponovo okrenite i verifikujte poslednji ID Transakcije."
      , vF = "ILI"
      , kF = "Odaberite jedan ID Transakcije na stranici istorije igre kako biste verifikovali ponovo."
      , AF = "Licencirano u\nUK i na Malti"
      , $F = "Sertifikovano od strane\nGA / BMM (UKGC)"
      , TF = {
        official: YM,
        nofound: XM,
        nofound_details: JM,
        expired: QM,
        expired_details: ZM,
        wrong: eF,
        wrong_details: tF,
        error: nF,
        error_details: iF,
        fake: sF,
        fake_details: rF,
        gentle: oF,
        reminder: aF,
        certified: lF,
        step1: cF,
        step1_details: uF,
        step2: fF,
        step2_details: dF,
        chars: pF,
        official_th: _F,
        th_only: hF,
        unstable: mF,
        unstable_details: gF,
        incompleted: bF,
        incompleted_return: yF,
        incompleted_or: vF,
        incompleted_history: kF,
        licensed_top: AF,
        certified_top: $F
    }
      , EF = "Officiella och äkta PG-spel"
      , wF = "Transaktions-ID hittades inte"
      , SF = "Återgå till spelhistoriksidan för att verifiera den senaste spelregistreringen."
      , OF = "Session har löpt ut"
      , PF = "Återgå till spelhistoriksidan för att verifiera den senaste spelregistreringen."
      , xF = "Felaktig parameter"
      , IF = "Återgå till spelhistoriksidan för att verifiera den senaste spelregistreringen."
      , CF = "Internt serverfel"
      , RF = "Uppdatera eller återgå till spelhistoriksidan för att verifiera den senaste spelregistreringen."
      , LF = "VARNING: Förfalskat spel upptäckt!"
      , NF = "Detta spel är INTE en officiell PG SOFT<sup>®</sup>-produkt! Vi rekommenderar starkt att du slutar spela omedelbart för din säkerhets skull."
      , DF = "Snäll påminnelse"
      , MF = "Förfalskade spel är inte certifierade och kan utföra säkerhetsrisker och finansiella hot. Använd den officiella verifieringswebbplatsen {link} för att se till att dina rättigheter är skyddade."
      , FF = "PG SOFT<sup>®</sup>-produkter har dubbelcertifierats och testats av de globala myndigheterna BMM och GA, som garanterar att RNG-algoritmen och implementering, spelmatematik och regler följer de högsta branschstandarderna för rättvisa och  integritet."
      , GF = "STEG 1:"
      , UF = "Välj ett Transaktions-ID."
      , jF = "STEG 2:"
      , BF = "Klicka på knappen Verifiera. Du omdirigeras automatiskt."
      , zF = "* Säkerställ att webbadressen matchar exakt."
      , VF = "Officiella och äkta PG-spel med Askmebet"
      , HF = "Endast verifierat av Askmebet Thailand."
      , KF = "Ostabil nätverksanslutning"
      , qF = "Uppdatera sidan och försök igen."
      , WF = "Verifiering ofullständig"
      , YF = "Snurra om och verifiera senaste transaktions-ID."
      , XF = "ELLER"
      , JF = "Välj ett av de transaktions-ID på spelhistoriksidan för att verifiera igen."
      , QF = "Licensierade i\nStorbritannien och Malta"
      , ZF = "Certifierat av GA /\nBMM (UKGC)"
      , eG = {
        official: EF,
        nofound: wF,
        nofound_details: SF,
        expired: OF,
        expired_details: PF,
        wrong: xF,
        wrong_details: IF,
        error: CF,
        error_details: RF,
        fake: LF,
        fake_details: NF,
        gentle: DF,
        reminder: MF,
        certified: FF,
        step1: GF,
        step1_details: UF,
        step2: jF,
        step2_details: BF,
        chars: zF,
        official_th: VF,
        th_only: HF,
        unstable: KF,
        unstable_details: qF,
        incompleted: WF,
        incompleted_return: YF,
        incompleted_or: XF,
        incompleted_history: JF,
        licensed_top: QF,
        certified_top: ZF
    }
      , tG = "เกม PG ของแท้อย่างเป็นทางการ"
      , nG = "ไม่พบ ID ธุรกรรม"
      , iG = "กรุณากลับไปยังหน้าประวัติเกมเพื่อยืนยันบันทึกเดิมพันล่าสุด"
      , sG = "เซสชันหมดอายุ"
      , rG = "กรุณากลับไปยังหน้าประวัติเกมเพื่อยืนยันบันทึกเดิมพันล่าสุด"
      , oG = "พารามิเตอร์ไม่ถูกต้อง"
      , aG = "กรุณากลับไปยังหน้าประวัติเกมเพื่อยืนยันบันทึกเดิมพันล่าสุด"
      , lG = "ข้อผิดพลาดเซิร์ฟเวอร์ภายใน"
      , cG = "กรุณารีเฟรชหรือกลับไปยังหน้าประวัติเกมเพื่อยืนยันบันทึกเดิมพันล่าสุด"
      , uG = "คำเตือน: ตรวจพบเกมปลอม!"
      , fG = "เกมนี้ไม่ใช่ผลิตภัณฑ์อย่างเป็นทางการของ PG SOFT<sup>®</sup>! เราขอแนะนำเป็นอย่างยิ่งให้คุณหยุดเล่นเกมนี้โดยทันทีเพื่อความปลอดภัยของคุณ"
      , dG = "คำเตือน"
      , pG = "เกมลอกเลียนแบบไม่ได้รับการรับรองและอาจก่อให้เกิดความเสี่ยงด้านความปลอดภัยและภัยคุกคามทางการเงิน กรุณาใช้เว็บไซต์ยืนยันอย่างเป็นทางการ {link} เพื่อให้มั่นใจว่าสิทธิ์ของคุณได้รับการคุ้มครอง"
      , _G = "ผลิตภัณฑ์ PG SOFT<sup>®</sup> ได้รับการรับรองและทดสอบอย่างเข้มงวดโดยหน่วยงาน BMM และ GA ระดับโลก เพื่อให้มั่นใจว่าอัลกอริทึม RNG และการนำไปใช้ คณิตศาสตร์ในเกม และกฎเกณฑ์เป็นไปตาม มาตรฐานอุตสาหกรรมสูงสุดเพื่อความยุติธรรมและความสมบูรณ์"
      , hG = "ขั้นที่ 1:"
      , mG = "เลือกหนึ่งใน ID ธุรกรรม"
      , gG = "ขั้นที่ 2:"
      , bG = "คลิกที่ปุ่มยืนยันระบบจะนำทางโดยอัตโนมัติ"
      , yG = "* กรุณาตรวจสอบว่า URL ตรงกัน"
      , vG = "เกม PG อย่างเป็นทางการและเป็นของแท้กับ Askmebet"
      , kG = "ตรวจสอบโดย Askmebet ประเทศไทยเท่านั้น"
      , AG = "การเชื่อมต่อเครือข่ายไม่เสถียร"
      , $G = "กรุณารีเฟรชหน้านี้และลองอีกครั้ง"
      , TG = "การยืนยันยังไม่สมบูรณ์"
      , EG = "หมุนซ้ำและยืนยัน ID ธุรกรรมล่าสุด"
      , wG = "หรือ"
      , SG = "เลือกหนึ่งใน ID ธุรกรรมในหน้าประวัติการเล่นเกมเพื่อยืนยันอีกครั้ง"
      , OG = "ลิขสิทธิ์ในสหราช\nอาณาจักรและมอลตา"
      , PG = "รับรองโดย GA / \nBMM (UKGC)"
      , xG = {
        official: tG,
        nofound: nG,
        nofound_details: iG,
        expired: sG,
        expired_details: rG,
        wrong: oG,
        wrong_details: aG,
        error: lG,
        error_details: cG,
        fake: uG,
        fake_details: fG,
        gentle: dG,
        reminder: pG,
        certified: _G,
        step1: hG,
        step1_details: mG,
        step2: gG,
        step2_details: bG,
        chars: yG,
        official_th: vG,
        th_only: kG,
        unstable: AG,
        unstable_details: $G,
        incompleted: TG,
        incompleted_return: EG,
        incompleted_or: wG,
        incompleted_history: SG,
        licensed_top: OG,
        certified_top: PG
    }
      , IG = "Resmi ve Orijinal PG Oyunları"
      , CG = "İşlem kimliği bulunamadı"
      , RG = "En son bahis kaydını doğrulamak için lütfen oyun geçmişi sayfasına dönün."
      , LG = "Oturum Süresi Doldu"
      , NG = "En son bahis kaydını doğrulamak için lütfen oyun geçmişi sayfasına dönün."
      , DG = "Hatalı Parametre"
      , MG = "En son bahis kaydını doğrulamak için lütfen oyun geçmişi sayfasına dönün."
      , FG = "Dahili Sunucu Hatası"
      , GG = "En son bahis kaydını doğrulamak için lütfen yenileyin veya oyun geçmişi sayfasına dönün."
      , UG = "DİKKAT: Sahte Oyun Tespit Edildi!"
      , jG = "Bu oyun resmi bir PG SOFT<sup>®</sup> ürünü DEĞİLDİR! Güvenliğiniz için derhal bu oyunu oynamayı bırakmanızı şiddetle tavsiye ederiz."
      , BG = "Nazik Hatırlatma"
      , zG = "Sahte oyunlar sertifikalı değildir ve güvenlik riskleri ve finansal tehditler oluşturabilir. Haklarınızın korunduğundan emin olmak için lütfen {link} resmi doğrulama web sitesini kullanın."
      , VG = "PG SOFT<sup>®</sup> ürünleri; RNG'nin algoritması ve uygulanması, oyun matematiği ve kurallarının adillik ve dürüstlük açısından en yüksek sektör standartlarını karşıladığından emin olunması amacıyla BMM ve GA adlı küresel yetkili kuruluşlar tarafından sıkı testlerden geçirilerek bu ürünlere çift sertifika verilmiştir."
      , HG = "ADIM 1:"
      , KG = "Bir İşlem Kimliği seçin."
      , qG = "ADIM 2:"
      , WG = "Doğrula düğmesine tıklayın. Otomatik olarak yönlendirilirsiniz."
      , YG = "* URL'nin tam olarak eşleştiğinden emin olun."
      , XG = "Askmebet ile Resmi ve Orijinal PG Oyunları"
      , JG = "Yalnızca Askmebet Tayland tarafından doğrulanmıştır."
      , QG = "İstikrarsız Ağ Bağlantısı"
      , ZG = "Lütfen bu sayfayı yenileyin ve tekrar deneyin."
      , eU = "Doğrulama Tamamlanmadı"
      , tU = "Yeniden döndürün ve en son İşlem ID’sini doğrulayın."
      , nU = "VEYA"
      , iU = "Tekrar doğrulamak için oyun geçmişi sayfasındaki İşlem ID’lerinden birini seçin."
      , sU = "Birleşik Krallık'ta\nve Malta'da lisanslanmıştır."
      , rU = "GA /\nBMM Onaylı (UKGC)"
      , oU = {
        official: IG,
        nofound: CG,
        nofound_details: RG,
        expired: LG,
        expired_details: NG,
        wrong: DG,
        wrong_details: MG,
        error: FG,
        error_details: GG,
        fake: UG,
        fake_details: jG,
        gentle: BG,
        reminder: zG,
        certified: VG,
        step1: HG,
        step1_details: KG,
        step2: qG,
        step2_details: WG,
        chars: YG,
        official_th: XG,
        th_only: JG,
        unstable: QG,
        unstable_details: ZG,
        incompleted: eU,
        incompleted_return: tU,
        incompleted_or: nU,
        incompleted_history: iU,
        licensed_top: sU,
        certified_top: rU
    }
      , aU = "Справжні й офіційні PG Ігри"
      , lU = "ID транзакції не знайдено"
      , cU = "Поверніться на сторінку історії гри, щоб перевірити останній запис ставки."
      , uU = "Сеанс закінчився"
      , fU = "Поверніться на сторінку історії гри, щоб перевірити останній запис ставки."
      , dU = "Неправильний параметр"
      , pU = "Поверніться на сторінку історії гри, щоб перевірити останній запис ставки."
      , _U = "Внутрішня помилка сервера"
      , hU = "Оновіть або поверніться на сторінку історії гри, щоб перевірити останній запис ставки."
      , mU = "ОБЕРЕЖНО: Виявлено підроблену гру!"
      , gU = "Ця гра не є офіційною продукцією PG SOFT<sup>®</sup>! Задля вашої безпеки, переконливо радимо вам негайно припинити грати."
      , bU = "Дружнє нагадування"
      , yU = "Підроблені ігри не сертифіковані та можуть становити загрозу безпеці й фінансові загрози. Скористайтеся офіційним вебсайтом для верифікації {link}, щоб переконатися в тому, що ваші права захищено."
      , vU = "Продукція PG SOFT<sup>®</sup> відповідає суворим вимогам подвійної сертифікації та пройшла перевірку органами BMM і GA, що гарантує відповідність алгоритму, реалізації ГВЧ, математики і правил найвищим галузевим стандартам чесності та сумлінності."
      , kU = "КРОК 1:"
      , AU = "Виберіть один з ID Транзакції"
      , $U = "КРОК 2:"
      , TU = "Натисніть на кнопку «Верифікувати». Вас буде автоматично перенаправлено."
      , EU = "* Переконайтеся у точній відповідності посилання URL."
      , wU = "Справжні й офіційні PG Ігри від Askmebet"
      , SU = "Підтверджено лише Askmebet Таїланд."
      , OU = "Нестабільне інтернет з'єднання"
      , PU = "Оновіть сторінку та спробуйте ще."
      , xU = "Верифікацію не завершено"
      , IU = "Виконайте повторне обертання й перевірте ID останньої транзакції."
      , CU = "АБО"
      , RU = "Виберіть один із ID Транзакції на сторінці історії гри для повторної перевірки."
      , LU = "Ліцензовано\nу Великій Британії \nта на Мальті"
      , NU = "Сертифіковано \nGA / BMM (UKGC)"
      , DU = {
        official: aU,
        nofound: lU,
        nofound_details: cU,
        expired: uU,
        expired_details: fU,
        wrong: dU,
        wrong_details: pU,
        error: _U,
        error_details: hU,
        fake: mU,
        fake_details: gU,
        gentle: bU,
        reminder: yU,
        certified: vU,
        step1: kU,
        step1_details: AU,
        step2: $U,
        step2_details: TU,
        chars: EU,
        official_th: wU,
        th_only: SU,
        unstable: OU,
        unstable_details: PU,
        incompleted: xU,
        incompleted_return: IU,
        incompleted_or: CU,
        incompleted_history: RU,
        licensed_top: LU,
        certified_top: NU
    }
      , MU = "باضابطہ اور حقیقی PG گیمز"
      , FU = "لین دین ID نہیں ملی"
      , GU = "تازہ ترین شرط کے ریکارڈ کی تصدیق کے لیے براہ کرم گیم ہسٹری کے صفحے پر واپس جائیں۔"
      , UU = "سیشن کی معیاد ختم ہوگئی"
      , jU = "تازہ ترین شرط کے ریکارڈ کی تصدیق کے لیے براہ کرم گیم ہسٹری کے صفحے پر واپس جائیں۔"
      , BU = "غلط پیرامیٹر"
      , zU = "تازہ ترین شرط کے ریکارڈ کی تصدیق کے لیے براہ کرم گیم ہسٹری کے صفحے پر واپس جائیں۔"
      , VU = "اندرونی سرور کی خرابی"
      , HU = "تازہ ترین شرط کے ریکارڈ کی تصدیق کے لیے براہ کرم ریفریش کریں یا گیم ہسٹری کے صفحے پر واپس جائیں۔"
      , KU = "انتباہ: جعلی کھیل کا انکشاف ہوا!"
      , qU = "یہ کھیل PG SOFT<sup>®</sup> کا باضابطہ پروڈکٹ نہیں ہے! ہم آپ کی حفاظت کے لئے زور دیتے ہیں کہ اسے فوراً کھیلنا بند کر دیں۔"
      , WU = "ہلکی یاد دہانی"
      , YU = "جعلی گیمز تصدیق شدہ نہیں ہیں اور ان سے سیکیورٹی اور مالی خطرات لاحق ہو سکتے ہیں۔ براہ کرم تصدیق کی باضابطہ ویب سائٹ {link} استعمال کریں تاکہ آپ کے حقوق محفوظ رہیں۔"
      , XU = "PG SOFT<sup>®</sup> پروڈکٹس کی عالمی حکام BMM اور GA کے ذریعے سختی سے دوہری تصدیق اور جانچ کی گئی ہے، اس بات کو یقینی بناتے ہوئے کہ RNG الگورتھم اور عمل درآمد، گیم میتھمیٹکس، اور قواعد انصاف اور دیانت کے اعلیٰ ترین صنعتی معیارات پر پورا اترتے ہیں۔"
      , JU = "مرحلہ 1:"
      , QU = "میں سے ایک کو منتخب کریں لین دین کی ID۔"
      , ZU = "مرحلہ 2:"
      , ej = "پر کلک کریں تصدیق کا بٹن۔ آپ کو خودکار طور پر ری ڈائریکٹ کیا جائے گا۔"
      , tj = "* یقینی بنائیں کہ URL بالکل مماثل ہے۔"
      , nj = "Askmebet کے ساتھ باضابطہ اور حقیقی PG گیمز"
      , ij = "صرف Askmebet تھائی لینڈ کے ذریعے تصدیق شدہ۔"
      , sj = "غیر مستحکم نیٹ ورک کنکشن"
      , rj = "براہ کرم اس پیج کو ریفریش کریں اور دوبارہ کوشش کریں۔"
      , oj = "تصدیق نامکمل ہے"
      , aj = "تازہ ترین لین دین کی ID کی توثیق کرنے کے لیے ری اسپن کریں۔"
      , lj = "یا"
      , cj = "دوبارہ تصدیق کرنے کے لیے گیم ہسٹری کے پیج میں لین دین کی ID میں سے ایک کو منتخب کریں۔"
      , uj = "UK اور مالٹا میں\nلائسنس شدہ"
      , fj = "GA / BMM (UKGC)\nسے سرٹیفائیڈ"
      , dj = {
        official: MU,
        nofound: FU,
        nofound_details: GU,
        expired: UU,
        expired_details: jU,
        wrong: BU,
        wrong_details: zU,
        error: VU,
        error_details: HU,
        fake: KU,
        fake_details: qU,
        gentle: WU,
        reminder: YU,
        certified: XU,
        step1: JU,
        step1_details: QU,
        step2: ZU,
        step2_details: ej,
        chars: tj,
        official_th: nj,
        th_only: ij,
        unstable: sj,
        unstable_details: rj,
        incompleted: oj,
        incompleted_return: aj,
        incompleted_or: lj,
        incompleted_history: cj,
        licensed_top: uj,
        certified_top: fj
    }
      , pj = "Rasmiy va haqiqiy PG oʻyinlari"
      , _j = "Tranzaksiya IDsi topilmadi"
      , hj = "Eng so'nggi tikish rekordini tekshirish uchun o'yin tarixi sahifasiga qayting."
      , mj = "Seans muddati tugadi"
      , gj = "Eng so'nggi tikish rekordini tekshirish uchun o'yin tarixi sahifasiga qayting."
      , bj = "Noto'g'ri Parametr"
      , yj = "Eng so'nggi tikish rekordini tekshirish uchun o'yin tarixi sahifasiga qayting."
      , vj = "Ichki server xatosi"
      , kj = "Eng so'nggi tikish rekordini tekshirish uchun sahifani yangilang yoki o'yin tarixi sahifasiga qayting."
      , Aj = "DIQQAT: Soxta Oʻyin Aniqlandi!"
      , $j = "Bu oʻyin rasmiy PG SOFT<sup>®</sup> mahsuloti EMAS! Xavfsizligingiz uchun oʻynashni darhol toʻxtatishni qatʼiy tavsiya qilamiz."
      , Tj = "Kamtarin Eslatma"
      , Ej = "Soxta o'yinlar sertifikatlanmagan va xavfsizlik va moliyaviy tahdidlarni keltirib chiqarishi mumkin. Huquqlaringiz himoyalanganiga ishonch hosil qilish uchun {link} rasmiy tekshirish veb-saytidan foydalaning."
      , wj = "PG SOFT<sup>®</sup> mahsulotlari RNG algoritmi va joriy etilishi, oʻyin matematikasi va qoidalarning adolat va halolligi boʻyicha eng yuqori sanoat standartlarini taʼminlovchi BMM va GA global idoralari tomonidan qat'iy ikki karra sertifikatlangan va sinovdan oʻtgan."
      , Sj = "1-QADAM:"
      , Oj = "Tranzaksiya IDdan birini tanlang."
      , Pj = "2-QADAM:"
      , xj = "Tasdiqlash tugmasini bosing. Siz avtomatik yoʻnaltirilasiz."
      , Ij = "* URL aniq mos kelishiga ishonch hosil qiling."
      , Cj = "Askmebet bilan rasmiy va haqiqiy PG oʻyinlari"
      , Rj = "Faqat Askmebet Tailand tomonidan tasdiqlangan."
      , Lj = "Tarmoqqa ulanish beqaror"
      , Nj = "Iltimos, sahifani yangilang va takror urinib ko‘ring."
      , Dj = "Tasdiqlash yakunlanmagan"
      , Mj = "Qayta aylantiring va so‘nggi tranzaksiya IDsini tasdiqlang."
      , Fj = "YOKI"
      , Gj = "Takror tasdiqlash uchun o‘yin tarixi sahifasida tranzaksiya IDsidan birini tanlang."
      , Uj = "Buyuk Britaniya\nva Maltada\nlitsenziyalangan"
      , jj = "GA / BMM (UKGC)\ntomonidan\nsertifikatlangan"
      , Bj = {
        official: pj,
        nofound: _j,
        nofound_details: hj,
        expired: mj,
        expired_details: gj,
        wrong: bj,
        wrong_details: yj,
        error: vj,
        error_details: kj,
        fake: Aj,
        fake_details: $j,
        gentle: Tj,
        reminder: Ej,
        certified: wj,
        step1: Sj,
        step1_details: Oj,
        step2: Pj,
        step2_details: xj,
        chars: Ij,
        official_th: Cj,
        th_only: Rj,
        unstable: Lj,
        unstable_details: Nj,
        incompleted: Dj,
        incompleted_return: Mj,
        incompleted_or: Fj,
        incompleted_history: Gj,
        licensed_top: Uj,
        certified_top: jj
    }
      , zj = "Trò chơi PG chính thức và chính hãng"
      , Vj = "Không tìm thấy mã giao dịch"
      , Hj = "Vui lòng quay trở lại trang lịch sử trò chơi để kiểm tra hồ sơ cược mới nhất."
      , Kj = "Phiên đã hết hạn"
      , qj = "Vui lòng quay trở lại trang lịch sử trò chơi để kiểm tra hồ sơ cược mới nhất."
      , Wj = "Tham số không chính xác"
      , Yj = "Vui lòng quay trở lại trang lịch sử trò chơi để kiểm tra hồ sơ cược mới nhất."
      , Xj = "Lỗi máy chủ nội bộ"
      , Jj = "Vui lòng làm mới hoặc quay trở lại trang lịch sử trò chơi để kiểm tra hồ sơ cược mới nhất."
      , Qj = "CẢNH BÁO: Phát hiện trò chơi giả mạo!"
      , Zj = "Trò chơi này KHÔNG phải là sản phẩm chính thức của PG SOFT<sup>®</sup>! Chúng tôi khuyến cáo bạn nên ngừng chơi ngay lập tức để đảm bảo an toàn."
      , e2 = "Lời nhắc nhở nhẹ nhàng"
      , t2 = "Trò chơi giả mạo không được chứng nhận và có thể gây rủi ro về bảo mật cũng như tài chính. Vui lòng sử dụng trang web xác minh chính thức {link} để đảm bảo rằng quyền lợi của bạn được bảo vệ."
      , n2 = "Sản phẩm của PG SOFT<sup>®</sup> đã được các cơ quan toàn cầu BMM và GA chứng nhận và kiểm tra nghiêm ngặt, đảm bảo rằng thuật toán RNG và cách thức triển khai, toán học trong trò chơi và các quy tắc đáp ứng tiêu chuẩn cao nhất trong ngành về công bằng và toàn vẹn."
      , i2 = "BƯỚC 1:"
      , s2 = "Chọn một Mã Giao Dịch."
      , r2 = "BƯỚC 2:"
      , o2 = "Nhấn nút Xác minh. Bạn sẽ được tự động chuyển hướng."
      , a2 = "* Đảm bảo URL khớp chính xác."
      , l2 = "Các trò chơi PG chính thức và chính hãng với Askmebet"
      , c2 = "Chỉ do Askmebet Thái Lan xác nhận."
      , u2 = "Kết Nối Mạng Không Ổn Định"
      , f2 = "Vui lòng làm mới trang này và thử lại."
      , d2 = "Xác Minh Chưa Hoàn Tất"
      , p2 = "Quay lại và xác minh ID Giao Dịch mới nhất."
      , _2 = "HOẶC"
      , h2 = "Chọn một trong các ID Giao Dịch trong trang lịch sử trò chơi để xác minh lại."
      , m2 = "Được cấp giấy\nphép tại nước Anh và Malta"
      , g2 = "Chứng nhận bởi GA /\nBMM (UKGC)"
      , b2 = {
        official: zj,
        nofound: Vj,
        nofound_details: Hj,
        expired: Kj,
        expired_details: qj,
        wrong: Wj,
        wrong_details: Yj,
        error: Xj,
        error_details: Jj,
        fake: Qj,
        fake_details: Zj,
        gentle: e2,
        reminder: t2,
        certified: n2,
        step1: i2,
        step1_details: s2,
        step2: r2,
        step2_details: o2,
        chars: a2,
        official_th: l2,
        th_only: c2,
        unstable: u2,
        unstable_details: f2,
        incompleted: d2,
        incompleted_return: p2,
        incompleted_or: _2,
        incompleted_history: h2,
        licensed_top: m2,
        certified_top: g2
    }
      , y2 = "PG官方正版游戏"
      , v2 = "交易单号未找到"
      , k2 = "请返回游戏历史记录页面，验证最新的投注记录。"
      , A2 = "会话超时"
      , $2 = "请返回游戏历史记录页面，验证最新的投注记录。"
      , T2 = "参数错误"
      , E2 = "请返回游戏历史记录页面，验证最新的投注记录。"
      , w2 = "系统错误"
      , S2 = "请刷新或返回游戏历史记录页面，验证最新的投注记录。"
      , O2 = "警告: 检测到盗版游戏！"
      , P2 = "此游戏并非PG SOFT<sup>®</sup>的官方产品！为了您的安全，我们强烈建议您立即停止游戏。"
      , x2 = "温馨提醒"
      , I2 = "盗版游戏未经过认证，会为您带来安全隐患和资金风险。请认准官方唯一验证网址 {link} 以保证您的权益。"
      , C2 = "PG SOFT<sup>®</sup>的产品已通过国际权威认证机构BMM和GA的双重严格认证和测试，确保了游戏的RNG算法和实施，所有游戏的数学运算和规则皆符合行业最高标准和要求，保证公正与公平。"
      , R2 = "步骤一 :"
      , L2 = "选择一个交易单号。"
      , N2 = "步骤二 :"
      , D2 = "点击验证按钮。页面将自动跳转"
      , M2 = "* 请确保URL的字符一致。"
      , F2 = "与Askmebet一起，畅玩PG官方正版游戏！"
      , G2 = "仅由泰国Askmebet验证。"
      , U2 = "网络连接不稳定"
      , j2 = "请刷新页面并重试。"
      , B2 = "验证未完成"
      , z2 = "重新旋转并验证最新的交易单号。"
      , V2 = "或"
      , H2 = "在游戏历史记录页面中选择一个交易单号以再次进行验证。"
      , K2 = "权威机构"
      , q2 = "公平认证"
      , W2 = {
        official: y2,
        nofound: v2,
        nofound_details: k2,
        expired: A2,
        expired_details: $2,
        wrong: T2,
        wrong_details: E2,
        error: w2,
        error_details: S2,
        fake: O2,
        fake_details: P2,
        gentle: x2,
        reminder: I2,
        certified: C2,
        step1: R2,
        step1_details: L2,
        step2: N2,
        step2_details: D2,
        chars: M2,
        official_th: F2,
        th_only: G2,
        unstable: U2,
        unstable_details: j2,
        incompleted: B2,
        incompleted_return: z2,
        incompleted_or: V2,
        incompleted_history: H2,
        licensed_top: K2,
        certified_top: q2
    }
      , Y2 = mv({
        legacy: !1,
        locale: "EN",
        warnHtmlMessage: !1,
        messages: {
            AR: xk,
            AZ: o0,
            BG: D0,
            BN: dA,
            CS: BA,
            DA: b$,
            DE: W$,
            EL: TT,
            EN: e1,
            ES: x1,
            ET: oE,
            FA: DE,
            FI: dw,
            FR: Bw,
            HI: bS,
            HU: WS,
            HY: TO,
            ID: eP,
            IT: xP,
            JA: ox,
            KO: Dx,
            LO: dI,
            LT: BI,
            MN: bC,
            MY: WC,
            NL: TR,
            NO: eL,
            PL: xL,
            PT: oN,
            RO: DN,
            RU: dD,
            SI: BD,
            SK: bM,
            SQ: WM,
            SH: TF,
            SV: eG,
            TH: xG,
            TR: oU,
            UK: DU,
            UR: dj,
            UZ: Bj,
            VI: b2,
            ZH: W2
        }
    })
      , Gd = lh(ek);
    Gd.use(Y2);
    Gd.mount("#app")
}
);
export default X2();
