﻿module ParserTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Interop
open FSharpLib.LocationCodeParser
open DataContracts

[<TestClass>]
type UnitTest() = 
    // ===== Lat/Long ===================================
    [<TestMethod>]
    member x.ParseLatLngHappyTest () = 
        let code = "=12,15"
        let lat = new Nullable<decimal>(12m)
        let lng = new Nullable<decimal>(15m)
        let expected = new LocationFilterMessage(Latitude = lat, Longitude = lng)
        let actual = parseLocationCode code false
        Assert.AreEqual(expected.Latitude, actual.Latitude)
        Assert.AreEqual(expected.Longitude, actual.Longitude)


    [<TestMethod>]
    member x.ParseLatLngWrongLatTest () = 
        let code = "=21,15"
        let lat = new Nullable<decimal>(12m)
        let lng = new Nullable<decimal>(15m)
        let expected = new LocationFilterMessage(Latitude = lat, Longitude = lng)
        let actual = parseLocationCode code false
        Assert.AreNotEqual(expected.Latitude, actual.Latitude)
        Assert.AreEqual(expected.Longitude, actual.Longitude)

    [<TestMethod>]
    member x.ParseLatLngWrongLngTest() = 
        let code = "=12,51"
        let lat = new Nullable<decimal>(12m)
        let lng = new Nullable<decimal>(15m)
        let expected = new LocationFilterMessage(Latitude = lat, Longitude = lng)
        let actual = parseLocationCode code false
        Assert.AreEqual(expected.Latitude, actual.Latitude)
        Assert.AreNotEqual(expected.Longitude, actual.Longitude)

     // ===== Cross-Feature ===================================
//    [<TestMethod>]
//    member x.parseLeftSideHappyHighwayNumberTest () = 
//        let code = "/H205"
//        let filter = new LocationFilterMessage()
//        let expected = new LocationFilterMessage(HighwayNumber = "H205")
//        let actual = parseLocationCode code false
//        Assert.AreEqual(expected.HighwayNumber, actual.HighwayNumber)

//    [<TestMethod>]
//    member x.parseLeftSideHappyRouteNumberTest1 () = 
//        let str = "I-5"
//        let filter = new LocationFilterMessage()
//        let expected = new LocationFilterMessage(RouteNumber = str)
//        let actual = parseLeftSide str filter
//        Assert.AreEqual(expected.RouteNumber, actual.RouteNumber)
//
//    [<TestMethod>]
//    member x.parseLeftSideHappyRouteNumberTest2 () = 
//        let str = "OR-20"
//        let filter = new LocationFilterMessage()
//        let expected = new LocationFilterMessage(RouteNumber = str)
//        let actual = parseLeftSide str filter
//        Assert.AreEqual(expected.RouteNumber, actual.RouteNumber)
//
//    [<TestMethod>]
//    member x.parseLeftSideHappyHighwayNumberTest3 () = 
//        let str = "US-205"
//        let filter = new LocationFilterMessage()
//        let expected = new LocationFilterMessage(RouteNumber = str)
//        let actual = parseLeftSide str filter
//        Assert.AreEqual(expected.RouteNumber, actual.RouteNumber)
//
//    [<TestMethod>]
//    member x.parseLeftSideNoMatchTest () = 
//        let str = "bananas!"
//        let filter = new LocationFilterMessage()
//        let expected = new LocationFilterMessage(Alias = str)
//        let actual = parseLeftSide str filter
//        Assert.AreEqual(expected.Alias, actual.Alias)
//
//    [<TestMethod>]
//    member x.parseRightSideHappyTest () = 
//        let str = "NB"
//        let filter = new LocationFilterMessage()
//        let expected = new LocationFilterMessage(CrossFeature = str)
//        let actual = parseRightSide str filter
//        Assert.AreEqual(expected.CrossFeature, actual.CrossFeature)

