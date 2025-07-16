/// <binding Clean='on-compile' ProjectOpened='default' />

import gulp from 'gulp'; // Gulp is required
import sass from 'gulp-dart-sass'; // Processes SASS files in to CSS files
import sassGlob from 'gulp-sass-glob'; //Allows use of wildcard @import in SASS files that isn't possible in regular SASS (e.g. '@import "vars/**/*.scss"')
import autoprefixer from 'gulp-autoprefixer'; // Autoprefixes CSS properties with browser specific prefixes when needed
import mmq from 'gulp-merge-media-queries'; // Merges duplicate media queries in the same file into 1 media query
import cssnano from 'gulp-cssnano'; // Minifies/nanofies CSS files
import concat from 'gulp-concat'; // Concatenates files by operating system newLine
import terser from 'gulp-terser'; // Minifies Javascript files (has ES6 support)
import sourcemaps from 'gulp-sourcemaps'; // Creates a diff of minified versions of files so debugging in the browser is still possible
import gulpMultiProcess from 'gulp-multi-process'; //Allows multiple gulp.task()'s to run in parallel on different cores - makes on-compile process somewhat faster
import notifier from 'node-notifier'; //For notifying errors to the user


//Tasks for building CSS ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
gulp.task('sass-build-site-save', function (done) { //Faster task to be ran when saving the SASS file - quicker than full mmq/cssnano process
    return gulp.src('./wwwrootSrc/css/site/*.scss')
        .pipe(sassGlob()) //Allow wildcard @import
        .pipe(sass())
        .on('error', swallowError) //This will swallow the error and stop task from terminating - Simon
        .pipe(autoprefixer())
        .pipe(concat('site.css'))
        .pipe(gulp.dest('./wwwroot/css/'))
        .pipe(concat('site.min.css'))
        .pipe(gulp.dest('./wwwroot/css/'));
});

gulp.task('sass-build-site-compile', function (done) {
    return gulp.src('./wwwrootSrc/css/site/*.scss')
        .pipe(sassGlob()) //Allow wildcard @import
        .pipe(sass())
        .on('error', swallowError) //This will swallow the error and stop task from terminating - Simon
        .pipe(autoprefixer())
        .pipe(concat('site.css'))
        .pipe(gulp.dest('./wwwroot/css/'))
        .pipe(sourcemaps.init())
        .pipe(mmq())
        .pipe(cssnano({ zindex: false, discardComments: { removeAll: true } }))
        .pipe(concat('site.min.css'))
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./wwwroot/css/'));
});

gulp.task('sass-build-vendor', function (done) {
    return gulp.src([
         './wwwrootSrc/css/vendor/*/*.scss'
        ,'./wwwrootSrc/css/vendor/*.scss'
        ,'./wwwrootSrc/css/vendor/*/*.css'
        ,'./wwwrootSrc/css/vendor/*.css'
    ])
        .pipe(sassGlob()) //Allow wildcard @import
        .pipe(sass())
        .on('error', swallowError) //This will swallow the error and stop task from terminating - Simon
        .pipe(autoprefixer())
        .pipe(concat('vendor.css'))
        .pipe(gulp.dest('./wwwroot/css/'))
        .pipe(sourcemaps.init())
        .pipe(mmq())
        .pipe(cssnano({ zindex: false, discardComments: { removeAll: true } }))
        .pipe(concat('vendor.min.css'))
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./wwwroot/css/'));
});

//Function for notifying sass compilation errors ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
function swallowError(error) {
    console.log(error.toString()) //Logs the error in console
    notifier.notify("SASS Build Error. \nCheck console output for details.") //sends out a OS notification
    this.emit('end')
}




//Tasks for building JS ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
gulp.task('js-build-vendor', function (done) {
    return gulp.src([ //Load in a specific order to handle dependencies
        './wwwrootSrc/js/vendor/jquery/jquery.min.js'
        , './wwwrootSrc/js/vendor/jquery/jquery.placeholder.min.js'
        , './wwwrootSrc/js/vendor/bootstrap/bootstrap.bundle.min.js' //NOTE: Only include bootstrap.bundle.js, not the bootstrap.js file. Otherwise will cause issues with drop-downs requiring a double-click to open. Make sure to not even have it in the folder, as otherwise the *.js will pick it up: https://stackoverflow.com/questions/24218663/avoid-having-to-double-click-to-toggle-bootstrap-dropdown
        , './wwwrootSrc/js/vendor/*/*.js' //Concatenate any other .js files that aren't a dependency to any others (gulp won't concatenate any other previously defined files twice)
        , './wwwrootSrc/js/vendor/*.js' //Concatenate any other .js files that aren't a dependency to any others (gulp won't concatenate any other previously defined files twice)
    ])
        .pipe(sourcemaps.init())
        .pipe(concat('vendor.js'))
        .pipe(gulp.dest('./wwwroot/js/'))
        .pipe(terser())
        .pipe(concat('vendor.min.js'))
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./wwwroot/js/'));
});

gulp.task('js-build-site', function (done) {
    return gulp.src([ //Specifically concatenate any .js files that other files will be dependent on first
         './wwwrootSrc/js/site/Apfco.js'
        , './wwwrootSrc/js/site/*/*.js' //Concatenate any other .js files that aren't a dependency to any others (gulp won't concatenate any other previously defined files twice)
        , './wwwrootSrc/js/site/*.js' //Concatenate any other .js files that aren't a dependency to any others (gulp won't concatenate any other previously defined files twice)
    ])
        .pipe(sourcemaps.init())
        .pipe(concat('site.js'))
        .pipe(gulp.dest('./wwwroot/js/'))
        .pipe(terser())
        .pipe(concat('site.min.js'))
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest('./wwwroot/js/'));
});


//Default & Compile Tasks ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
gulp.task('build-all-multi-core', function (done) {
    return gulpMultiProcess(['sass-build-site-compile', 'sass-build-vendor', 'js-build-vendor', 'js-build-site'], done);
});

gulp.task('on-compile', gulp.series('build-all-multi-core')); //Run all builds parallel

gulp.task('default', function () {
    gulp.watch('./wwwrootSrc/css/site/*.scss', gulp.series('sass-build-site-save'));
    gulp.watch('./wwwrootSrc/css/site/*/*.scss', gulp.series('sass-build-site-save'));
    gulp.watch('./wwwrootSrc/css/site/*/*/*.scss', gulp.series('sass-build-site-save'));
    gulp.watch('./wwwrootSrc/css/vendor/*/*.scss', gulp.series('sass-build-vendor'));
    gulp.watch('./wwwrootSrc/css/vendor/*.scss', gulp.series('sass-build-vendor'));
    gulp.watch('./wwwrootSrc/css/vendor/*/*.css', gulp.series('sass-build-vendor'));
    gulp.watch('./wwwrootSrc/css/vendor/*.css', gulp.series('sass-build-vendor'));
    gulp.watch('./wwwrootSrc/js/vendor/*.js', gulp.series('js-build-vendor'));
    gulp.watch('./wwwrootSrc/js/site/*.js', gulp.series('js-build-site'));
});