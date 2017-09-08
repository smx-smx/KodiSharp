#include <stdio.h>
#include <stdlib.h>
#include <libgen.h>
#include <string.h>

char *remove_ext(const char *mystr) {
	char *retstr, *lastdot;
	if (mystr == NULL)
		return NULL;
	if ((retstr = (char *)malloc(strlen(mystr) + 1)) == NULL)
		return NULL;
	strcpy(retstr, mystr);
	lastdot = strrchr(retstr, '.');
	if (lastdot != NULL)
		*lastdot = '\0';
	return retstr;
}

char *my_basename(const char *path){
	char *cpy = strdup(path);
	char *ret = basename(cpy);
	ret = strdup(ret);
	free(cpy);
	return ret;
}

char *my_dirname(const char *path){
	char *cpy = strdup(path);
	char *ret = dirname(cpy);
	ret = strdup(ret);
	free(cpy);
	return ret;
}
