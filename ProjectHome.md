# crystalwall是使用C#开发的一个.NET权限框架 #

crystalwall框架使用xml配置文件以及元数据属性的方式进行权限控制。crystalwallk框架对原有系统是非侵入的，你无须改变原有系统的代码，只需简单的配置既能实现多方面的权限控制。

crystalwall框架使用了“限制点”、“权限包含”、“权限集”、“资源”、“安全上下文”等概念进行.NET权限的控制。crystalwall是一个可扩展的.NET框架，开发人员能够根据实际情况对其进行完全的扩充。


.NET权限系统组成
对于任何一个.NET权限系统来说，基本上都分为两个部分：
1、认证：对用户所声称的身份进行合法性验证。
2、授权：对用户声称的身份能够访问的资源进行控制
由于当前开源认证系统非常丰富，例如统一身份认证OAuther，在CrystalWall V1.0版本中不包含认证部分。1.0版本仅仅针对授权进行开发。

---

## crystalwall C#权限框架支持以下功能 ##
  1. 静态指定的权限，在程序中固定的例如某模块的权限
  1. 动态指定的权限，通过你自己编写的方法获得权限进行权限限制
  1. 任意层次的树状组织机构内权限资源的限制，例如
```
集团公司 
   --**省分公司

     --销售部

       --销售总监

       --业务员

     --财务部

       --财务总监

       --会计

       --出纳

     --生产部

       --车间主任
```
  1. 针对ASP.NET Page页面中控件的权限，包括不显示权限
  1. 如果以上都不满足条件，你需要在任意代码处控制权限，crystalwall则支持手工代码的方式进行权限控制，你只需要在你想要进行权限控制的地方使用++运算符即可
  1. crystalwall框架不是基于角色的权限控制框架，但角色也是crystalwall控制的目标之一，Role角色在crystalwall框架中被认为是普通的权限集的一个名称