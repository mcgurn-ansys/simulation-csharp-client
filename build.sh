#!/bin/bash -e
RUN_TEST=0
CLEAN=0
CONFIGURATION=Release
PUBLISH_DIR=publish
SRC_DIR=src/SimulationCSharpClient

die() {
  echo "ERROR: ${@}" 1>&2
  exit 1
}

show_help() {
  echo "USAGE: build.sh [options]"
  echo ""
  echo "OPTIONS:"
#   echo "  --coverage"
#   echo "    Calculate code coverage of unit tests, enables --test."
  echo "  --debug"
  echo "    Build with debugging information."
  echo "  -h | --help"
  echo "    Show this help page."
  echo "  --pub-dir=<publish_directory>"
  echo "    Publish output to specified directory, relative to src/BuildFileProcessor [${PUBLISH_DIR}]"
  echo "  --test"
  echo "    Build and execute unit tests."
  echo "  --clean"
  echo "    Remove all build artifacts"
  echo ""
}

for i in "${@}"; do
  case "${i}" in
    # debug
    --debug)
    CONFIGURATION=Debug
    ;;
    # help
    -h|--help)
    show_help
    exit 0
    ;;
    # pub-dir
    --pub-dir=*)
    PUBLISH_DIR="${i#*=}"
    ;;
    # testing
    --test)
    RUN_TEST=1
    ;;
    # clean
    --clean)
    CLEAN=1
    ;;
    # bad argument
    *)
    die "Unknown option '${i}'"
    ;;
  esac
done

rm -rf src/*/${PUBLISH_DIR}
dotnet publish -c ${CONFIGURATION} -o ${PUBLISH_DIR} ${SRC_DIR}
if [[ "${RUN_TEST}" == "1" ]]; then

  # before testing make sure that the version numbers match
  propertiesVersionContent=$(<version.properties)
  propertiesVersion=${propertiesVersionContent#*=}
  csprojVersion=`grep -oPm1 "(?<=<Version>)[^<]+" src/SimulationCSharpClient/SimulationCSharpClient.csproj`
  if [[ "${propertiesVersion}" != "${csprojVersion}" ]]; then
    die "The csproj version '${csprojVersion}' must match the version.properties '${propertiesVersion}'"
  fi

  for d in test/*; do
    dotnet test ${d}
  done
fi
if [[ "${CLEAN}" == "1" ]]; then
  dotnet clean
  rm -rf src/*/bin src/*/obj src/*/${PUBLISH_DIR}
  rm -rf test/*/bin test/*/obj
fi
