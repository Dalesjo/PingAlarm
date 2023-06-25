#!/bin/bash

SCRIPTDIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
EXECUTABLE=${SCRIPTDIR}/PingAlarm;
USERNAME=`id -un`
GROUPNAME=`id -gn`

echo "test"

echo $SCRIPTDIR
echo $EXECUTABLE

chmod +x $EXECUTABLE

cat << EOF | sudo tee /etc/systemd/system/PingAlarm.service
[Unit]
Description="PingAlarm from  https://github.com/Dalesjo/PingAlarm"

[Service]
User=${USERNAME}
Group=${GROUPNAME}
Type=notify
WorkingDirectory=${SCRIPTDIR}
ExecStart=${EXECUTABLE}

[Install]
WantedBy=multi-user.target

EOF

sudo systemctl daemon-reload
sudo systemctl enable PingAlarm
sudo systemctl start PingAlarm
sudo systemctl status PingAlarm
