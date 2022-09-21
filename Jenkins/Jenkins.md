# Jenkins安装

​	Jenkins是一个[开源软件](https://baike.baidu.com/item/开源软件/8105369?fromModule=lemma_inlink)项目，是基于[Java](https://baike.baidu.com/item/Java/85979?fromModule=lemma_inlink)开发的一种[持续集成](https://baike.baidu.com/item/持续集成/6250744?fromModule=lemma_inlink)工具，用于监控持续重复的工作，旨在提供一个开放易用的软件平台，使软件项目可以进行持续集成 。

​	持续集成是一种软件开发实践，即团队开发成员经常集成他们的工作，通常每个成员每天至少集成一次，也就意味着每天可能会发生多次集成。每次集成都通过自动化的构建（包括[编译](https://baike.baidu.com/item/编译/1258343)，发布，自动化测试）来验证，从而尽早地发现集成错误。

官网：https://www.jenkins.io

## Linux安装

~~~shell
#java jdk安装
#下载
wget --no-check-certificate --no-cookies --header "Cookie: oraclelicense=accept-securebackup-cookie" http://download.oracle.com/java/17/latest/jdk-17_linux-x64_bin.rpm
#授权限
chmod +x jdk-17_linux-x64_bin.rpm
#安装
rpm -ivh jdk-17_linux-x64_bin.rpm
~~~

~~~shell
#官网下载jenkins.war ,切换到当前目录执行
# 下载jenkins.war ,切换到当前目录 执行
#修改配置文件之前,不要后台启动 关闭窗体就停止
java -jar jenkins.war --httpPort=8081
#后台运行 --后台启动,窗体关闭了,也在后台启动
nohup java -jar jenkins.war --httpPort=9999 &
#输入jps 查看当前jenkins启动的进程号
ps ef|grep jenkins 查询当前启动的jenkins的进程号
#如果要关闭,kill -9 端口号

~~~

启动完成之后打开地址 http://43.142.140.211:8081 然后根据提示找到密码输入就OK

![](F:\MyGithub\Jenkins\img\1.png)

修改下面操作,更新使用, 如果更新不成功,则多更新几次 修改下列文件之后,记得要给两个文件权限 

1、修改根目录文 件/home/jenkins/hudson.model.UpdateCenter.xml

```shell
<sites>
  <site>
    <id>default</id>
<url>http://mirror.xmission.com/jenkins/updates/upd
ate-center.json</url>
  </site>
</sites>
```



# .NETCore&Jenkins

1. https://github.com/  创建仓库
   ![](F:\MyGithub\Jenkins\img\3.png)
2. Visual Studio Installer 安装git组件选择 GitHub扩展
   ![](F:\MyGithub\Jenkins\img\2.png)

3. 新建空白解决方案，同步git仓库地址

   ![]()![4](F:\MyGithub\Jenkins\img\4.png)