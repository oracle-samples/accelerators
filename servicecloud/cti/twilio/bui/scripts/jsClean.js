var fs = require('fs');
var fileUtils = require('./fileUtils');

var webJFiles = new fileUtils.getFiles(__dirname + '/../web', /(\.js|\.js\.map)$/, /^.*[\/|\\](libs\/)|(module\/console\/assets\/)/);

var skipFileRegex = /^.*[\/|\\](specRunnerBoot)|(karma\.conf)\.js$/i;

function jsCleanFiles(name, fileList){
    console.log('__________________________________________________')
    console.log('REMOVE TRANSPILED JAVASCRIPT FILES FOR ' + name)
    for (var index in fileList) {
        var file = fileList[index];

        if(skipFileRegex.test(file)) {
            console.log("skipping file -> " + file);
        }
        else {
            console.log("removing file -> " + file);
            fs.unlinkSync(fileList[index]);
        }
    }
    console.log('');
    console.log('Removed ' + fileList.length + ' file(s) from ' + name.toLowerCase() );
}

jsCleanFiles('WEB', webJFiles);







