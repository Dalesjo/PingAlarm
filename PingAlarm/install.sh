#!/bin/bash

SCRIPTDIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)
EXECUTABLE=${SCRIPTDIR}/PingAlarm
USERNAME=$(id -un)
GROUPNAME=$(id -gn)

echo "test"

echo $SCRIPTDIR
echo $EXECUTABLE

# make binary executable (required)
chmod +x $EXECUTABLE

# enable PingAlarm to create network sockets (needed for ping)
sudo setcap cap_net_raw+ep PingAlarm
sudo setcap 'cap_net_bind_service=+ep' PingAlarm

cat <<EOF | sudo tee /etc/systemd/system/ping-alarm.service
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
sudo systemctl enable ping-alarm
sudo systemctl restart ping-alarm
sudo systemctl status ping-alarm
