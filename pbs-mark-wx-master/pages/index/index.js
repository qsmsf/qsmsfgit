//index.js
const util = require("../../utils/util.js");
//播放的视频或者音频的ID
var playingID = -1;
var types = ["1","41"];
var page = 1;//页码
var allMaxtime = 0;//全部 最大时间
var myMaxtime = 0;

//1->全部;41->视频;10->图片;29->段子;31->声音;
var DATATYPE = {
    ALLDATATYPE : "1",
    MYDATATYPE : "41"
};

Page({
  //页面的初始化数据
  data:{
    allDataList:[{sid:1,
      recordNo:'NS00001',
      title:'南山区斗殴',
      content:'南山大道1009号2人打架斗殴',   avatarUrl:'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
      creatorName:'马光磊',
      createTime:'2018-03-20'},
      {
        sid: 2,
        recordNo: 'NS00002',
        title: '蛇口盗窃',
        content: '蛇口区舜天大厦6楼盗窃电脑',
        avatarUrl: 'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
        creatorName: '马光磊',        
        createTime: '2018-03-21'
      }],
    myDataList: [{
      sid: 1,
      recordNo: 'NS00001',
      title: '南山区斗殴',
      content: '南山大道1009号2人打架斗殴',
      avatarUrl: 'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
      creatorName: '马光磊',
      createTime: '2018-03-20'
    }],
    topTabItems:["全部","我参与的"],
    currentTopItem: "0",
    swiperHeight:"0",
    modalShowStyle: '',
    title:'',
    content:''
  },
  //页面初始化 options为页面跳转所带来的参数
  //生命周期函数，监听页面加载
  onLoad:function(options){
    this.refreshNewData();
  },
  //生命周期函数-监听页面初次渲染完毕
  onReady:function(){
    var that = this;
     wx.getSystemInfo({
       success: function(res) {
         that.setData({
            swiperHeight: (res.windowHeight-37)
         });
       }
     })
  },
  //切换顶部标签
  switchTab:function(e){
    this.setData({
      currentTopItem:e.currentTarget.dataset.idx
    });
    //如果需要加载数据
    if (this.needLoadNewDataAfterSwiper()) {
      this.refreshNewData();
    }
  },

  //swiperChange
  bindChange:function(e){
    var that = this;
    that.setData({
      currentTopItem:e.detail.current
    });

    //如果需要加载数据
    if (this.needLoadNewDataAfterSwiper()) {
      this.refreshNewData();
    }
  },
  
  //刷新数据
  refreshNewData:function(){
    //加载提示框
    util.showLoading();

    var that = this;
    /*
    var parameters = 'a=list&c=data&type='+types[this.data.currentTopItem];
    console.log("parameters = "+parameters);
    util.request(parameters,function(res){
      page = 1;
      that.setNewDataWithRes(res,that);
      setTimeout(function(){
          util.hideToast();
          wx.stopPullDownRefresh();
        },1000);
      });
      */
  },
  
  //监听用户下拉动作
  onPullDownRefresh:function(){
    this.refreshNewData();
  },

  //滚动后需不要加载数据
  needLoadNewDataAfterSwiper:function(){

    switch(types[this.data.currentTopItem]) {
      //全部
      case DATATYPE.ALLDATATYPE:
        return this.data.allDataList.length > 0 ? false : true;
        
      //视频
      case DATATYPE.MYDATATYPE:
        return this.data.myDataList.length > 0 ? false : true;
             
      default:
        break;
    }

    return false;
  },
  //设置新数据
  setNewDataWithRes:function(res,target){
    switch(types[this.data.currentTopItem]) {
      //全部
      case DATATYPE.ALLDATATYPE:
        allMaxtime = res.data.info.maxtime;
        target.setData({
          allDataList: res.data.list
        });
        break;
      //视频
      case DATATYPE.MYDATATYPE:
        myMaxtime = res.data.info.maxtime;
        target.setData({
          myDataList: res.data.list
        });
        break;      
      default:
        break;
    }
  },

  //加载更多操作
  loadMoreData:function(){
    console.log("加载更多");
    //加载提示框
    util.showLoading();

    var that = this;
    /*
    var parameters = 'a=list&c=data&type='+types[this.data.currentTopItem] + "&page="+(page+1) + "&maxtime="+this.getMaxtime();
    console.log("parameters = "+parameters);
    util.request(parameters,function(res){
      page += 1;
      that.setMoreDataWithRes(res,that);
      setTimeout(function(){
          util.hideToast();
          wx.stopPullDownRefresh();
        },1000);
      });
      */
  },

  //获取最大时间
  getMaxtime:function(){
    switch(types[this.data.currentTopItem]) {
      //全部
      case DATATYPE.ALLDATATYPE:
        return allMaxtime ;
      //视频
      case DATATYPE.MYDATATYPE:
        return myMaxtime ;
      
      default:
        return 0;
    }
  },
  //设置加载更多的数据
  setMoreDataWithRes(res,target) {
    switch(types[this.data.currentTopItem]) {
      //全部
      case DATATYPE.ALLDATATYPE:
        allMaxtime = res.data.info.maxtime;
        target.setData({
          allDataList: target.data.allDataList.concat(res.data.list)
        });
        break;
      //视频
      case DATATYPE.MYDATATYPE:
        videoMaxtime = res.data.info.maxtime;
        target.setData({
          myDataList: target.data.videoDataList.concat(res.data.list)
        });
        console.log(array);
        break;      
      default:
        break;
    }
  },  
  showModal: function () {
    console.log('aaa')
    this.setData({
      modalShowStyle: 'opacity:1;pointer-events:auto;'
    })
  },
  touchAddNew: function () {
    if (this.data.title === '') {
      wx.showToast({
        title: '标题不能为空',
        icon: 'success',
        duration: 2000,
        success: function () {
        }
      })
    } else {      
      var newList = this.data.allDataList
      newList.push({
        sid: 1,
        title: this.data.title,
        content: this.data.content,
        avatarUrl: 'http://wx.qlogo.cn/mmhead/PiajxSqBRaEKmHURbdqMQTISFGicUWZ7XwuVNbTzjsjWp61hyTfwJKQA/132',
        creatorName: '马光磊',
        createTime: '2018-03-20'
      })
      this.setData({
        allDataList: newList,
        modalShowStyle: '',
        title: '',
        content: ''
      })
    }
  },
  titleInput: function(e) {
    let that = this
    let title = e.detail.value

    this.setData({
      title: title
    })
  },
  contentInput:function(e) {
    let that = this
    let content = e.detail.value

    this.setData({
      content: content
    })
  },
  touchCancel: function () {
    this.setData({
      modalShowStyle: ''
    })
  },  
  goDetail: function(e){
    console.log(e)
    wx.navigateTo({
      url: 'detail?id='+e.currentTarget.dataset.item.sid
    })
  }
 
})
