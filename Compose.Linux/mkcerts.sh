#!/bin/bash
openssl req -config wut.conf -new -x509 -sha256 -newkey rsa:2048 -nodes \
    -keyout wut.key -days 3650 -out wut.crt 
openssl pkcs12 -export -out wut.pfx -inkey wut.key -in wut.crt 
certutil -d sql:$HOME/.pki/nssdb -D -n 'balticserver - Warsaw University of Technology'
pk12util -d sql:$HOME/.pki/nssdb -i wut.pfx -n 'balticserver - Warsaw University of Technology'
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n 'balticserver - Warsaw University of Technology' -i wut.crt
cp wut.pfx ../Baltic.Server/
certutil -d sql:$HOME/.pki/nssdb -L
