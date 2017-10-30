var fs = require('fs');

module.exports = {
    getFiles: function(dir, includeRegex, excludeRegex) {

        if(includeRegex && typeof(includeRegex) === 'string') {
            includeRegex = new RegExp(includeRegex.toString());
        }

        if(excludeRegex && typeof(excludeRegex) === 'string') {
            excludeRegex = new RegExp(excludeRegex.toString());
        }

        var fileList = [];

        var getFileList = function(dir, includeRegex, excludeRegex, fileList){
            var fileList = fileList || [];

            var files = fs.readdirSync(dir);
            for(var i in files) {
                if(!files.hasOwnProperty(i))
                    continue;

                var name = dir + '/' + files[i];
                if(fs.statSync(name).isDirectory()) {
                    getFileList(name, includeRegex, excludeRegex, fileList);
                } else {
                    if(includeRegex) {
                        // Apply regex filter if provided
                        if(includeRegex.test(name)) {
                            if(excludeRegex){
                                if(!excludeRegex.test(name)){
                                    fileList.push(name);
                                }
                            }else {
                                fileList.push(name);
                            }
                        }
                    }
                    else {
                        // Otherwise, return all files
                        fileList.push(name);
                    }
                }
            }

            return fileList;
        };

        return getFileList(dir, includeRegex, excludeRegex, fileList);
    }
};
