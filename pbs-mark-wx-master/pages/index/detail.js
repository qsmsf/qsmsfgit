const util = require("../../utils/util.js");
var amapFile = require('../../utils/amap-wx.js');
var data_id = 0;//帖子的ID

var app = getApp()
Page({
  data:{
    item: {
      sid: 1,
      recordNo: 'NS00001',
      recordLoc: '深圳市南山区南山大道1009号',
      recordType: '寻衅滋事',
      title: '南山区斗殴',
      content: '2人打架斗殴', avatarUrl: 'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
      creatorName: '马光磊',      
      createTime: '2018-03-20',
      locName: '',
      locDesp: ''
    },
    traceList: [{
      sid: 1,
      uuid: 'ttt',
      name: '血迹1',
      traceType: '血迹',
      tqbw: '左手掌',
      disp: '伤者左手掌中部提取',
      creatorName: '马光磊',
      avatarUrl: 'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
      image1: 'http://img.pconline.com.cn/images/upload/upc/tx/itbbs/1009/28/c1/5345810_1285667432311_1024x1024.jpg',
      width: 400,
      height: 300,
      createTime: '2018-03-20 18:00:43',
      barcode: '1001'
    }],
    hotcomemnt_hidden:false,
    dataList: [],
    index: 0,
    topTabItems: ["痕迹采集", "案件信息"],
    swiperHeight: "0",
    currentTopItem: "0",
    location: "点击添加位置",
    locationStyle: '',
    content: '',
    loading: false, // 更新地理位置加载状态
    recordType: [{ id: "1001", msg: "盗窃" }, { id: "1002", msg: "抢劫" }]
  },
  onReady: function () {
    var that = this;
    wx.getSystemInfo({
      success: function (res) {
        that.setData({
          swiperHeight: (res.windowHeight - 30)
        });
      }
    })
  },
  onLoad:function(options){
    data_id = options.id;
    //页面初始化 options为页面跳转所带来的参数
    //this.refreshNewData();   

  },
  bindPickerChange: function (e) {
    // 页面隐藏
    this.setData({
      index: e.detail.value
    })
  },

  //切换顶部标签
  switchTab: function (e) {
    this.setData({
      currentTopItem: e.currentTarget.dataset.idx
    });    
  },

  //swiperChange
  bindChange: function (e) {
    var that = this;
    that.setData({
      currentTopItem: e.detail.current
    });
  },
  addTrace: function (e) {
    let that = this;
    wx.scanCode({
      onlyFromCamera: true,
      success: (res) => {
        var tmplist = this.data.traceList;        
        tmplist.push({
          sid: 1,
          uuid: 'ttt',          
          name: '',
          traceType: '指纹',
          tqbw: '门把手',
          disp: '大门内门把手提取',
          creatorName: '马光磊',
          avatarUrl: 'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
          image1:'',
          width: 0,
          height: 0,
          createTime: '2018-03-20 18:00:43',
          barcode: res.result})
        that.setData({
          traceList: tmplist
        })

      }
    })
  },
  choosePic: function(e){
    wx.chooseImage({
      count: 9, // 默认9
      sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
      sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
      success: function (res) {
        // 返回选定照片的本地文件路径列表，tempFilePath可以作为img标签的src属性显示图片
        var tempFilePaths = res.tempFilePaths
        var tempFiles = res.tempFiles
      }
    })
  },
  rescan: function(e){

  },
  relocate: function(e){
    wx.scanCode({
      onlyFromCamera: true,
      success: (res) => {     
        console.log(e)   
        console.log(res)
      }
    })
  },
  relocate: function () {
    this.setData({
      loading: true
    })
    
    var that = this;
    var item = that.data.item
    var amap = new amapFile.AMapWX({ key: '8ebbe699d71eed6674889848604e411a' });

    amap.getRegeo({
      success: function (data) {
        console.log(data)
        //成功回调
        wx.showToast({
          title: '定位成功',
          icon: 'success',
          duration: 1000
        })
        item.locName = data[0].name
        item.locDesp = data[0].desc
        that.setData({
          item: item,
          loading: false
        })
      }
    })

  }    
  
})