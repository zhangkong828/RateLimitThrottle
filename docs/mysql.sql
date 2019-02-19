CREATE TABLE `ratelimit_item` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Prefix` varchar(50) NOT NULL COMMENT '前缀',
  `PolicyType` int(11) NOT NULL COMMENT '策略类型  1 IP, 2 Client',
  `Value` varchar(100) NOT NULL,
  `Type` int(11) NOT NULL COMMENT '类型\n1，白名单\n2，黑名单\n3，策略',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;


CREATE TABLE `ratelimit_rule` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Endpoint` varchar(50) NOT NULL COMMENT '请求动词和路径\nget:/api/values\n*:/api/values\n*',
  `Period` varchar(10) NOT NULL COMMENT '限流周期： 1s, 1m, 1h',
  `Limit` bigint(20) NOT NULL COMMENT '最大请求数',
  `PolicyId` int(11) NOT NULL COMMENT '策略Id',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;
