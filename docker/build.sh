# Get script's directory
get_script_dir () {
     SOURCE="${BASH_SOURCE[0]}"
     # While $SOURCE is a symlink, resolve it
     while [ -h "$SOURCE" ]; do
          DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
          SOURCE="$( readlink "$SOURCE" )"
          # If $SOURCE was a relative symlink (so no "/" as prefix, need to resolve it relative to the symlink base directory
          [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE"
     done
     DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
     echo "$DIR"
}
DIR=$(get_script_dir)

function __error_handing__(){
    local last_status_code=$1;
    local error_line_number=$2;
    echo 1>&2 "[$(date +'%Y-%m-%d %H:%M:%S')][$0][FATAL] Error - exited with status $last_status_code at line $error_line_number";
    perl -slne 'if($.+5 >= $ln && $.-4 <= $ln){ $_="$. $_"; s/$ln/">" x length($ln)/eg; s/^\D+.*?$/\e[1;31m$&\e[0m/g;  print}' -- -ln="$error_line_number" "$0"
}
trap  '__error_handing__ $? $LINENO' ERR

set -eEuo pipefail

# Common functions
function log_fatal() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')][$0][FATAL]" "$@" >&2
}

function die() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')][$0][FATAL]" "$@" >&2
  exit 1
}

function info() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')][$0][INFO]" "$@" >&2
}

function debug() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')][$0][DEBUG]" "$@" >&2
}

function die_with_usage() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')][$0][FATAL]" "$@" >&2
  usage
  exit 1
}

function usage() {
  # [--verbose|-v] [--with-value val] [optional_arg]
    cat 1>&2 <<EOF
Usage: $(basename "$0") [--help|-h] argument1

Builds the executable using docker

Arguments:

None

Options:
                 
-h | --help          Command help
EOF

#[optional_arg]   Optional, this is optional.
#-v | --verbose       Verbose mode
#--with-value [val]   A value to set
}

POSITIONAL=()
# VERBOSE=false
# WITH_VALUE=""
while (( $# > 0 )); do
    case "${1}" in
        --) shift; POSITIONAL+=( "$@" ); break 2 ;; # signals end of flags/switches, rest is arguments hereon
        -h|--help) usage; exit 0 ;;
        # -v|--verbose)
        #     VERBOSE=true
        #     shift # shift once since flags have no values
        # ;;
        # -w|--with-value)
        #     numOfArgs=1 # number of switch arguments
        #     if (( $# < numOfArgs + 1 )); then
        #         shift $#
        #     else
        #         WITH_VALUE="${2}"
        #         shift $((numOfArgs + 1)) # shift 'numOfArgs + 1' to bypass switch and its value
        #     fi
        # ;;
        *) # unknown flag/switch
            POSITIONAL+=("${1}")
            shift
        ;;
    esac
done

set -- "${POSITIONAL[@]}" # restore positional params

#Parse positional arguments left after options
if [ "$#" -ne 0 ]; then
  die_with_usage "Illegal number of parameters!"
fi

cd "$DIR/.." || die "Failed to change directory to project root"

[[ ! -f "BroDisplaySetup.csproj" ]] && die "Failed to find 'BroDisplaySetup.csproj' (not in project root?)"

docker build -t "khbrodisplaysetup_builder:latest" . && docker run -v "$(pwd -P)/bin:/app/bin" --rm khbrodisplaysetup_builder:latest