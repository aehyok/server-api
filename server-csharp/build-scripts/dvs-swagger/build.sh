#! /bin/sh
set -eu

BASE_DIR=$(cd `dirname $0`; pwd)

function show_usage
{
    echo "Usage: $0 <label>"
    echo "Example:"
    echo "    sh $0 1.0.0.001"
    exit 1
}

while getopts ":" optname
do
    case $optname in
        *)
            show_usage
            exit 0
            ;;
    esac
done

if [ $# -lt 1 ]
then
    show_usage
    exit
fi

VERSION=$1
BasicName=dvs-swagger
SPECS="/usr/src/packages/SPECS"
SOURCES="/usr/src/packages/SOURCES"
SRPMS="/usr/src/packages/SRPMS"

rm -rf ${SOURCES}/${BasicName}.*

cd $BASE_DIR

mkdir -p ${SOURCES}/${BasicName}.${VERSION}

cp -ar ${BasicName}/* ${SOURCES}/${BasicName}.${VERSION}/
cp -ar ${BasicName}/appsettings.json ${SOURCES}/${BasicName}.appsettings.json
cp -ar ${BasicName}.service ${SOURCES}/

cd ${SOURCES}
tar -cvf ${BasicName}.${VERSION}.tar.gz ${BasicName}.${VERSION}



sVERSION=${VERSION%.*}
release=${VERSION##*.}
cp $BASE_DIR/${BasicName}.spec $SPECS/${BasicName}.spec
sed -i "s/_VERSION_/$sVERSION/g" $SPECS/${BasicName}.spec
sed -i "s/_RELEASE_/$release/g" $SPECS/${BasicName}.spec

rpmbuild -bb $SPECS/${BasicName}.spec

rm -rf ${SOURCES}/${BasicName}.*
